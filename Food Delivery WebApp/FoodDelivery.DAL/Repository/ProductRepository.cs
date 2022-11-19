namespace FoodDelivery.DAL.Repository
{
    using EF;
    using Entities;
    using Interfaces;
    using System.Linq;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class ProductRepository : IRepository<Product>
    {
        readonly DataContext db;

        public ProductRepository(DataContext db)
        {
            this.db = db;
        }

        public void Create(Product entity)
        {
            db.Products.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Product entity)
        {
            db.Products.Remove(entity);
            db.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var found = await db.Products.FindAsync(id);
            db.Products.Remove(found);
            await db.SaveChangesAsync();
        }

        public Product Get(int id)
        {
            return db.Products.Find(id);
        }

        public IQueryable<Product> GetAll()
        {
            return db.Products;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await db.Products.ToListAsync();
        }

        public void Update(Product entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
