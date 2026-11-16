namespace KTBE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OrderDetail")]
    public partial class OrderDetail
    {
        [Key]
        public int ID { get; set; }

        public int ProductID { get; set; }

        public int OrderID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? TotalPrice { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }
    }
}
