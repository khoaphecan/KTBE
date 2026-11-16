namespace KTBE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Order")]
    public partial class Order
    {
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public int OrderID { get; set; }

        public int CustomerID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentStatus { get; set; }

        public string PaymentMethod { get; set; }

        public string ShippingMethod { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        public string OrderStatus { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
