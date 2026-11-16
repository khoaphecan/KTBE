using System;
using System.Linq;
using System.Web.Mvc;
using KTBE.Models;

namespace KTBE.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Account/Login
        public ActionResult Login()
        {
            if (Session["UserRole"] != null && Session["UserRole"].ToString().Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Orders", new { area = "Admin" });
            }
            return View();
        }

        // POST: Admin/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null && (user.UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) || user.UserRole.Trim().Equals("A", StringComparison.OrdinalIgnoreCase)))
            {
                Session["Username"] = user.Username;
                Session["AdminUser"] = user.Username;
                Session["UserRole"] = "Admin";
                return RedirectToAction("Index", "Orders", new { area = "Admin" });
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác hoặc bạn không có quyền Admin.";
            return View();
        }

        // GET: Admin/Account/Logout
        public ActionResult Logout()
        {
            Session.Remove("AdminUser");
            Session.Remove("Username");
            Session.Remove("UserRole");
            return RedirectToAction("Login", "Account", new { area = "Admin" });
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
