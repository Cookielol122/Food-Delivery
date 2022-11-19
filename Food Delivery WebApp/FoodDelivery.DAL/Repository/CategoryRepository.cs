namespace FoodDelivery.DAL.Repository
{
    using EF;
    using Entities;
    using Interfaces;
    using System.Linq;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class CategoryRepository : IRepository<Category>
    {
        readonly DataContext db;

        public CategoryRepository(DataContext db)
        {
            this.db = db;
        }

        public void Create(Category entity)
        {
            db.Categories.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Category entity)
        {
            db.Categories.Remove(entity);
            db.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var found = await db.Categories.FindAsync(id);
            db.Categories.Remove(found);
            await db.SaveChangesAsync();
        }

        public Category Get(int id)
        {
            return db.Categories.Find(id);
        }

        public IQueryable<Category> GetAll()
        {
            return db.Categories;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await db.Categories.ToListAsync();
        }

        public void Update(Category entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
