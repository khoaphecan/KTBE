using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KTBE.Models;

namespace KTBE.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Home/Index
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        // GET: Home/ProductDetail/5
        public ActionResult ProductDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Hệ thống bán hàng trực tuyến FASTOR.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Liên hệ nhóm phát triển.";
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
