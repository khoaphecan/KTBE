namespace KTBE.Models.ViewModel
{
    using System.Collections.Generic;
    using System.Linq;

    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();

        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }

        public void AddItem(Product product, int quantity = 1)
        {
            if (product == null) return;
            var item = items.FirstOrDefault(i => i.ProductID == product.ProductID);
            if (item == null)
            {
                items.Add(new CartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    ProductImage = product.ProductImage,
                    UnitPrice = product.ProductPrice,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity += quantity;
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    items.Remove(item);
                }
            }
        }

        public void RemoveItem(int productId)
        {
            items.RemoveAll(i => i.ProductID == productId);
        }

        public void Clear()
        {
            items.Clear();
        }

        public decimal TotalAmount
        {
            get { return items.Sum(i => i.TotalPrice); }
        }

        public int TotalQuantity
        {
            get { return items.Sum(i => i.Quantity); }
        }
    }
}
