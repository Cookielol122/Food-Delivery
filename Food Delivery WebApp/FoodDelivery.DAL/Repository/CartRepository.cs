namespace FoodDelivery.DAL.Repository
{
    using EF;
    using Entities;
    using Interfaces;
    using System.Linq;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class CartRepository : IRepository<ShoppingCart>
    {
        readonly DataContext db;

        public CartRepository(DataContext db)
        {
            this.db = db;
        }

        public void Create(ShoppingCart entity)
        {
            db.ShoppingCarts.Add(entity);
            int r = db.SaveChanges();
        }

        public void Delete(ShoppingCart entity)
        {
            db.ShoppingCarts.Remove(entity);
            db.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var found = await db.ShoppingCarts.FindAsync(id);
            db.ShoppingCarts.Remove(found);
            await db.SaveChangesAsync();
        }

        public ShoppingCart Get(int id)
        {
            return db.ShoppingCarts.Find(id);
        }

        public IQueryable<ShoppingCart> GetAll()
        {
            return db.ShoppingCarts;
        }

        public async Task<List<ShoppingCart>> GetAllAsync()
        {
            return await db.ShoppingCarts.ToListAsync();
        }

        public void Update(ShoppingCart entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
