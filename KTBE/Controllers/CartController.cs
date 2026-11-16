using System.Web.Mvc;
using KTBE.Models;
using KTBE.Models.ViewModel;

namespace KTBE.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // GET: Cart
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }

        // GET or POST: Cart/AddToCart/5
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            if (quantity <= 0) quantity = 1;
            var product = db.Products.Find(id);
            if (product != null)
            {
                GetCartService().GetCart().AddItem(product, quantity);
            }
            return RedirectToAction("Index");
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            if (quantity <= 0)
            {
                GetCartService().GetCart().RemoveItem(id);
            }
            else
            {
                GetCartService().GetCart().UpdateQuantity(id, quantity);
            }
            return RedirectToAction("Index");
        }

        // GET: Cart/Remove/5
        public ActionResult Remove(int id)
        {
            GetCartService().GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }

        // GET: Cart/Clear
        public ActionResult Clear()
        {
            GetCartService().Clear();
            return RedirectToAction("Index");
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
