namespace FoodDelivery.DAL.Repository
{
    using EF;
    using Entities;
    using Interfaces;
    using System.Linq;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class OrderRepository : IRepository<Order>
    {
        readonly DataContext db;

        public OrderRepository(DataContext db)
        {
            this.db = db;
        }

        public void Create(Order entity)
        {
            db.Orders.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Order entity)
        {
            db.Orders.Remove(entity);
            db.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var found = await db.Orders.FindAsync(id);
            db.Orders.Remove(found);
            await db.SaveChangesAsync();
        }

        public Order Get(int id)
        {
            return db.Orders.Find(id);
        }

        public IQueryable<Order> GetAll()
        {
            return db.Orders;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await db.Orders.ToListAsync();
        }

        public void Update(Order entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
