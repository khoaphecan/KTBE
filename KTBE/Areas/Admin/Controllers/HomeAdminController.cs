using System;
using System.Linq;
using System.Web.Mvc;
using KTBE.Models;

namespace KTBE.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/HomeAdmin
        public ActionResult Index()
        {
            ViewBag.TotalOrders = db.Orders.Count();
            ViewBag.TotalCustomers = db.Customers.Count();
            ViewBag.TotalProducts = db.Products.Count();
            ViewBag.TotalCategories = db.Categories.Count();

            return View();
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
