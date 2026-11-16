using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KTBE.Models;
using KTBE.Models.ViewModel;
using PagedList;

namespace KTBE.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Orders
        public ActionResult Index(string customerName, string paymentStatus, DateTime? orderDate, int? page)
        {
            var orders = db.Orders.Include(o => o.Customer).AsQueryable();

            // 1. Lọc theo Tên khách hàng (nếu có)
            if (!string.IsNullOrEmpty(customerName))
            {
                customerName = customerName.Trim();
                orders = orders.Where(o => o.Customer.CustomerName.Contains(customerName));
            }

            // 2. Lọc theo Trạng thái thanh toán (nếu có)
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                paymentStatus = paymentStatus.Trim();
                orders = orders.Where(o => o.PaymentStatus == paymentStatus);
            }

            // 3. Lọc theo Ngày đặt hàng (nếu có)
            if (orderDate.HasValue)
            {
                var targetDate = orderDate.Value.Date;
                orders = orders.Where(o => DbFunctions.TruncateTime(o.OrderDate) == targetDate);
            }

            // Sắp xếp đơn hàng mới nhất lên đầu
            orders = orders.OrderByDescending(o => o.OrderDate);

            // Phân trang 15 đơn/trang
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var pagedOrders = orders.ToPagedList(pageNumber, pageSize);

            // Danh sách trạng thái thanh toán cho dropdown
            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Tất cả trạng thái --" },
                new SelectListItem { Value = "Chưa thanh toán", Text = "Chưa thanh toán" },
                new SelectListItem { Value = "Thanh toán tiền mặt", Text = "Thanh toán tiền mặt" },
                new SelectListItem { Value = "Đã thanh toán", Text = "Đã thanh toán" }
            };
            ViewBag.PaymentStatusList = new SelectList(statusOptions, "Value", "Text", paymentStatus);

            var model = new OrderSearchVM
            {
                CustomerName = customerName,
                PaymentStatus = paymentStatus,
                OrderDate = orderDate,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Orders = pagedOrders
            };

            return View(model);
        }

        // GET: Admin/Orders/DetailedOrder/5
        public ActionResult DetailedOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Admin/Orders/UpdateStatus/5
        public ActionResult UpdateStatus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            ViewBag.PaymentStatusList = new SelectList(new[] { "Chưa thanh toán", "Thanh toán tiền mặt", "Đã thanh toán" }, order.PaymentStatus);
            ViewBag.OrderStatusList = new SelectList(new[] { "Chờ thanh toán", "Đang xử lý", "Đang vận chuyển", "Đã giao", "Đã hủy" }, order.OrderStatus);

            return View(order);
        }

        // POST: Admin/Orders/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, string paymentStatus, string orderStatus)
        {
            var order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.PaymentStatus = paymentStatus;
            order.OrderStatus = orderStatus;

            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("DetailedOrder", new { id = order.OrderID });
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
