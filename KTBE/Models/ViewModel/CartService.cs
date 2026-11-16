namespace KTBE.Models.ViewModel
{
    using System.Web;

    public class CartService
    {
        private const string CartSessionKey = "Cart";
        private readonly HttpSessionStateBase session;

        public CartService(HttpSessionStateBase session)
        {
            this.session = session;
        }

        public Cart GetCart()
        {
            var cart = session[CartSessionKey] as Cart;
            if (cart == null)
            {
                cart = new Cart();
                session[CartSessionKey] = cart;
            }
            return cart;
        }

        public void Clear()
        {
            session.Remove(CartSessionKey);
        }
    }
}
