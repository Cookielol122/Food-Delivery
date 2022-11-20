namespace FoodDelivery.BLL.Services
{
    using DAL.UOF;
    using DAL.Entities;
    using DAL.Interfaces;
    using FoodDelivery.BLL.Dto;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FoodDeliveryService : Interfaces.IFoodDeliveryService
    {
        private readonly IUnitOfWork Db;

        public FoodDeliveryService(string connectStr)
        {
            Db = new UnitOfWork(connectStr);
        }

        #region Category service:
        public void Insert(CategoryDto dto)
        {
            Category category = new Category
            {
                Name = dto.Name,
                Photo = dto.Photo,
                TagName = dto.TagName
            };
            Db.Categories.Create(category);
        }

        public void Update(CategoryDto dto)
        {
            Category category = new Category
            {
                Name = dto.Name,
                Photo = dto.Photo,
                TagName = dto.TagName
            };
            Db.Categories.Update(category);
        }

        public void Delete(CategoryDto dto)
        {
            Category category = new Category
            {
                Name = dto.Name,
                Photo = dto.Photo,
                TagName = dto.TagName
            };
            Db.Categories.Delete(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await Db.Categories.DeleteAsync(id);
        }

        public IEnumerable<CategoryDto> ReadCategories()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CategoryDto> ReadCategoriesAsync()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Cart service:
        public void Insert(CartDto dto)
        {
            throw new System.NotImplementedException();
        }

        public void Update(CartDto dto)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(CartDto dto)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteCartAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CartDto> ReadCart()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CartDto> ReadCartAsync()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Order service:
        public void Insert(OrderDto dto)
        {
            throw new System.NotImplementedException();
        }

        public void Update(OrderDto dto)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(OrderDto dto)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteOrderAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<OrderDto> ReadOrders()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<OrderDto> ReadOrdersAsync()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Product service:
        public void Insert(ProductDto dto)
        {
            throw new System.NotImplementedException();
        }

        public void Update(ProductDto dto)
        {
            throw new System.NotImplementedException();
        }
        public void Delete(ProductDto dto)
        {
            throw new System.NotImplementedException();
        }
        public void DeleteProductAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ProductDto> ReadProducts()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ProductDto> ReadProductsAsync()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
