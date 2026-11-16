namespace KTBE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Category")]
    public partial class Category
    {
        public Category()
        {
            this.Products = new HashSet<Product>();
        }

        [Key]
        public int CategoryID { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
