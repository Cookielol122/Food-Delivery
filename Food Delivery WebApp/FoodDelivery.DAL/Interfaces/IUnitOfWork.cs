namespace FoodDelivery.DAL.Interfaces
{
    using Entities;

    public interface IUnitOfWork : System.IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<Order> Orders { get; }
        IRepository<ShoppingCart> ShoppingCarts { get; }
        IRepository<Category> Categories { get; }
    }
}
