namespace KTBE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public int ProductID { get; set; }

        public int CategoryID { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductDecription { get; set; }

        public decimal ProductPrice { get; set; }

        public string ProductImage { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
