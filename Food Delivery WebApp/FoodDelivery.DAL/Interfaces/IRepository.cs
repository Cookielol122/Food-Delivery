namespace FoodDelivery.DAL.Interfaces
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IRepository<IEntity> where IEntity : class
    {
        IEntity Get(int id);
        void Delete(IEntity entity);
        Task DeleteAsync(int id);
        void Update(IEntity entity);
        void Create(IEntity entity);
        IQueryable<IEntity> GetAll();
        Task<List<IEntity>> GetAllAsync();
    }
}
