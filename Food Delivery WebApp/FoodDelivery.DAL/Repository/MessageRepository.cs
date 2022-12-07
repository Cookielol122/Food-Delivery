namespace FoodDelivery.DAL.Repository
{
    using EF;
    using Entities;
    using Interfaces;
    using System.Linq;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class MessageRepository : IRepository<Message>
    {
        readonly DataContext db;

        public MessageRepository(DataContext db)
        {
            this.db = db;
        }

        public void Create(Message entity)
        {
            db.Messages.Add(entity);
            int r = db.SaveChanges();
        }

        public void Delete(Message entity)
        {
            db.Messages.Remove(entity);
            db.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var found = await db.Messages.FindAsync(id);
            db.Messages.Remove(found);
            await db.SaveChangesAsync();
        }

        public Message Get(int id)
        {
            return db.Messages.Find(id);
        }

        public IQueryable<Message> GetAll()
        {
            return db.Messages;
        }

        public async Task<List<Message>> GetAllAsync()
        {
            return await db.Messages.ToListAsync();
        }

        public void Update(Message entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
