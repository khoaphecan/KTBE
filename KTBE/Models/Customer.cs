namespace KTBE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Customer")]
    public partial class Customer
    {
        public Customer()
        {
            this.Orders = new HashSet<Order>();
        }

        [Key]
        public int CustomerID { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(15)]
        public string CustomerPhone { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerAddress { get; set; }

        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [ForeignKey("Username")]
        public virtual User User { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
