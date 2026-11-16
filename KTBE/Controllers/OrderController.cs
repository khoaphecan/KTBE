using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KTBE.Models;
using KTBE.Models.ViewModel;

namespace KTBE.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        private static readonly List<string> ValidStatuses = new List<string>
        {
            "Chờ thanh toán",
            "Đang xử lý",
            "Đang vận chuyển",
            "Đã giao",
            "Đã hủy"
        };

        // GET: Order/Checkout
        public ActionResult Checkout()
        {
            // 1. Kiểm tra đăng nhập
            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            // 2. Kiểm tra giỏ hàng
            var cart = new CartService(Session).GetCart();
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // 3. Lấy thông tin Customer
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null)
            {
                var user = db.Users.Find(username);
                if (user != null)
                {
                    customer = new Customer
                    {
                        Username = user.Username,
                        CustomerName = user.Username,
                        CustomerPhone = "0900000000",
                        CustomerAddress = "N/A"
                    };
                    db.Customers.Add(customer);
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            ViewBag.Cart = cart;
            return View(customer);
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string customerName, string customerPhone, string shippingAddress, string shippingMethod, string paymentMethod)
        {
            // 1. Kiểm tra đăng nhập
            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            // 2. Kiểm tra giỏ hàng
            var cartService = new CartService(Session);
            var cart = cartService.GetCart();
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // 3. Lấy hoặc cập nhật thông tin Customer
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null)
            {
                customer = new Customer
                {
                    Username = username,
                    CustomerName = !string.IsNullOrWhiteSpace(customerName) ? customerName.Trim() : username,
                    CustomerPhone = !string.IsNullOrWhiteSpace(customerPhone) ? customerPhone.Trim() : "0900000000",
                    CustomerAddress = !string.IsNullOrWhiteSpace(shippingAddress) ? shippingAddress.Trim() : "N/A"
                };
                db.Customers.Add(customer);
                db.SaveChanges();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(customerName)) customer.CustomerName = customerName.Trim();
                if (!string.IsNullOrWhiteSpace(customerPhone)) customer.CustomerPhone = customerPhone.Trim();
                if (!string.IsNullOrWhiteSpace(shippingAddress)) customer.CustomerAddress = shippingAddress.Trim();
                db.SaveChanges();
            }

            // 4. Địa chỉ giao hàng
            string finalAddress = !string.IsNullOrWhiteSpace(shippingAddress) ? shippingAddress.Trim() : customer.CustomerAddress;
            if (string.IsNullOrWhiteSpace(finalAddress))
            {
                finalAddress = "N/A";
            }

            // 5. Tạo đơn hàng mới
            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = cart.TotalAmount,
                PaymentStatus = "Chưa thanh toán",
                PaymentMethod = !string.IsNullOrWhiteSpace(paymentMethod) ? paymentMethod.Trim() : "Thanh toán khi nhận hàng (COD)",
                ShippingMethod = !string.IsNullOrWhiteSpace(shippingMethod) ? shippingMethod.Trim() : "Giao hàng tiêu chuẩn",
                ShippingAddress = finalAddress,
                OrderStatus = "Chờ thanh toán"
            };
            db.Orders.Add(order);
            db.SaveChanges();

            // 6. Tạo chi tiết đơn hàng
            foreach (var item in cart.Items)
            {
                var detail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                db.OrderDetails.Add(detail);
            }
            db.SaveChanges();

            // 7. Xóa giỏ hàng
            cartService.Clear();

            // 8. Chuyển hướng sang trang Đơn hàng của tôi
            return RedirectToAction("MyOrder", "Order");
        }

        // GET: Order/MyOrder
        public ActionResult MyOrder(string searchTerm, string status, string expandStatus)
        {
            // 1. Kiểm tra đăng nhập
            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("MyOrder", "Order") });
            }

            // 2. Lấy thông tin Customer
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null)
            {
                var user = db.Users.Find(username);
                if (user != null)
                {
                    customer = new Customer
                    {
                        Username = user.Username,
                        CustomerName = user.Username,
                        CustomerPhone = "0900000000",
                        CustomerAddress = "N/A"
                    };
                    db.Customers.Add(customer);
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            // 3. Truy vấn tất cả đơn hàng của đúng khách hàng này
            var baseQuery = db.Orders
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .Where(o => o.CustomerID == customer.CustomerID);

            int totalUserOrders = baseQuery.Count();

            // 4. Tìm kiếm theo Mã đơn hàng hoặc Tên sản phẩm
            var filteredQuery = baseQuery;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                filteredQuery = filteredQuery.Where(o =>
                    o.OrderID.ToString().Contains(searchTerm) ||
                    o.OrderDetails.Any(od => od.Product.ProductName.Contains(searchTerm))
                );
            }

            // 5. Xác định tab trạng thái hiện tại
            if (string.IsNullOrEmpty(status))
            {
                status = "Tất cả đơn";
            }

            var orderGroups = new List<OrderStatusGroup>();

            if (status.Equals("Tất cả đơn", StringComparison.OrdinalIgnoreCase))
            {
                // Gom nhóm theo từng trạng thái chuẩn
                foreach (var st in ValidStatuses)
                {
                    var ordersInStatus = filteredQuery
                        .Where(o => o.OrderStatus == st)
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();

                    if (ordersInStatus.Count > 0)
                    {
                        bool isExpanded = (!string.IsNullOrEmpty(expandStatus) && expandStatus.Equals(st, StringComparison.OrdinalIgnoreCase));
                        int takeCount = isExpanded ? 50 : 2;

                        orderGroups.Add(new OrderStatusGroup
                        {
                            StatusName = st,
                            TotalCount = ordersInStatus.Count,
                            Orders = ordersInStatus.Take(takeCount).ToList(),
                            IsExpanded = isExpanded
                        });
                    }
                }

                // Nếu có các đơn mang trạng thái khác ngoài danh sách chuẩn
                var otherOrders = filteredQuery
                    .Where(o => !ValidStatuses.Contains(o.OrderStatus))
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                if (otherOrders.Count > 0)
                {
                    bool isExpanded = (!string.IsNullOrEmpty(expandStatus) && expandStatus.Equals("Khác", StringComparison.OrdinalIgnoreCase));
                    int takeCount = isExpanded ? 50 : 2;

                    orderGroups.Add(new OrderStatusGroup
                    {
                        StatusName = "Khác",
                        TotalCount = otherOrders.Count,
                        Orders = otherOrders.Take(takeCount).ToList(),
                        IsExpanded = isExpanded
                    });
                }
            }
            else
            {
                // Xem riêng 1 trạng thái cụ thể
                var ordersInStatus = filteredQuery
                    .Where(o => o.OrderStatus == status)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                bool isExpanded = (!string.IsNullOrEmpty(expandStatus) && expandStatus.Equals(status, StringComparison.OrdinalIgnoreCase));
                int takeCount = isExpanded ? 50 : 2;

                orderGroups.Add(new OrderStatusGroup
                {
                    StatusName = status,
                    TotalCount = ordersInStatus.Count,
                    Orders = ordersInStatus.Take(takeCount).ToList(),
                    IsExpanded = isExpanded
                });
            }

            var model = new MyOrderVM
            {
                SearchTerm = searchTerm,
                ActiveStatus = status,
                ExpandedStatus = expandStatus,
                TotalUserOrders = totalUserOrders,
                OrderGroups = orderGroups
            };

            return View(model);
        }

        // GET: Order/OrderDetail/5
        public ActionResult OrderDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("OrderDetail", "Order", new { id = id }) });
            }

            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null)
            {
                return HttpNotFound();
            }

            var order = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(od => od.Product.Category))
                .FirstOrDefault(o => o.OrderID == id && o.CustomerID == customer.CustomerID);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Order/BuyAgain/5
        public ActionResult BuyAgain(int id)
        {
            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Orders
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .FirstOrDefault(o => o.OrderID == id && o.CustomerID == customer.CustomerID);

            if (order == null)
            {
                return HttpNotFound();
            }

            var cart = new CartService(Session).GetCart();
            foreach (var detail in order.OrderDetails)
            {
                if (detail.Product != null)
                {
                    // Thêm vào Giỏ hàng với số lượng = 1
                    cart.AddItem(detail.Product, 1);
                }
            }

            return RedirectToAction("Index", "Cart");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
