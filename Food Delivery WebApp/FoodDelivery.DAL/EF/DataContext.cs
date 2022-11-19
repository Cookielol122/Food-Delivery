namespace FoodDelivery.DAL.EF
{
    using Entities;
    using System.Data.Entity;

    public class DataContext : DbContext
    {
        public DataContext(string connection)
            : base(connection)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
    }
}
