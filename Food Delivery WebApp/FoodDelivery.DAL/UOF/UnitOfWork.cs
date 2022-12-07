namespace FoodDelivery.DAL.UOF
{
    using FoodDelivery.DAL.Entities;
    using Repository;
    using Interfaces;
    using EF;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext Db;
        private ProductRepository productRepo;
        private OrderRepository orderRepo;
        private CartRepository cartRepo;
        private CategoryRepository categoryRepo;
        private MessageRepository messageRepo;

        public UnitOfWork(string conn)
        {
            Db = new DataContext(conn);
        }

        public IRepository<Product> Products
        {
            get
            {
                if (productRepo == null)
                    productRepo = new ProductRepository(Db);
                return productRepo;
            }
        }

        public IRepository<Order> Orders
        {
            get
            {
                if (orderRepo == null)
                    orderRepo = new OrderRepository(Db);
                return orderRepo;
            }
        }

        public IRepository<ShoppingCart> ShoppingCarts
        {
            get
            {
                if (cartRepo == null)
                    cartRepo = new CartRepository(Db);
                return cartRepo;
            }
        }

        public IRepository<Category> Categories
        {
            get
            {
                if (categoryRepo == null)
                    categoryRepo = new CategoryRepository(Db);
                return categoryRepo;
            }
        }

        public IRepository<Message> Messages
        {
            get
            {
                if (messageRepo == null)
                    messageRepo = new MessageRepository(Db);
                return messageRepo;
            }
        }


        #region Dispose:
        bool disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                    Db.Dispose();

                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
        #endregion
    }
}
