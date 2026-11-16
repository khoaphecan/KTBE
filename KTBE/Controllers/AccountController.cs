using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using KTBE.Models;
using KTBE.Models.ViewModel;

namespace KTBE.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Account/Login
        public ActionResult Login(string returnUrl)
        {
            if (Session["Username"] != null)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("MyOrder", "Order");
            }

            return View(new LoginVM { ReturnUrl = returnUrl });
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = db.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Username, false);
                Session["Username"] = user.Username;
                Session["UserRole"] = user.UserRole;

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                if (user.UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) || user.UserRole.Trim().Equals("A", StringComparison.OrdinalIgnoreCase))
                {
                    Session["AdminUser"] = user.Username;
                    return RedirectToAction("Index", "Orders", new { area = "Admin" });
                }

                return RedirectToAction("MyOrder", "Order");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác.";
            return View(model);
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View(new RegisterVM());
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string username = model.Username.Trim();
            var existingUser = db.Users.Find(username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.");
                return View(model);
            }

            var user = new User
            {
                Username = username,
                Password = model.Password,
                UserRole = "Customer"
            };
            db.Users.Add(user);

            var customer = new Customer
            {
                Username = user.Username,
                CustomerName = model.CustomerName.Trim(),
                CustomerPhone = model.CustomerPhone.Trim(),
                CustomerEmail = !string.IsNullOrWhiteSpace(model.CustomerEmail) ? model.CustomerEmail.Trim() : null,
                CustomerAddress = !string.IsNullOrWhiteSpace(model.CustomerAddress) ? model.CustomerAddress.Trim() : null
            };
            db.Customers.Add(customer);

            db.SaveChanges();

            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // GET: Account/ProfileInfo
        public ActionResult ProfileInfo()
        {
            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("ProfileInfo", "Account") });
            }

            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null)
            {
                customer = new Customer
                {
                    Username = username,
                    CustomerName = username,
                    CustomerPhone = "0900000000",
                    CustomerAddress = "N/A"
                };
                db.Customers.Add(customer);
                db.SaveChanges();
            }

            return View(customer);
        }

        // POST: Account/ProfileInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProfileInfo(Customer model)
        {
            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer != null)
            {
                customer.CustomerName = model.CustomerName;
                customer.CustomerPhone = model.CustomerPhone;
                customer.CustomerEmail = model.CustomerEmail;
                customer.CustomerAddress = model.CustomerAddress;
                db.SaveChanges();
                ViewBag.Message = "Cập nhật hồ sơ thành công!";
            }

            return View(customer);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
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
