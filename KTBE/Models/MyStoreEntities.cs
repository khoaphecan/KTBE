namespace KTBE.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class MyStoreEntities : DbContext
    {
        public MyStoreEntities()
            : base("name=MyStoreEntities")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
