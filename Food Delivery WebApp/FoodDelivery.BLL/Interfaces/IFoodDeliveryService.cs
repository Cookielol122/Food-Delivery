namespace FoodDelivery.BLL.Interfaces
{
    using Dto;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFoodDeliveryService
    {
        #region Category:
        void Insert(CategoryDto model);
        void Update(CategoryDto model);
        void Delete(CategoryDto model);
        Task DeleteCategoryAsync(int id);
        IEnumerable<CategoryDto> ReadCategories();
        Task<IEnumerable<CategoryDto>> ReadCategoriesAsync();
        #endregion

        #region Cart:
        void Insert(CartDto model);
        void Update(CartDto model);
        void Delete(CartDto model);
        Task DeleteCartAsync(int id);
        IEnumerable<CartDto> ReadCart();
        Task<IEnumerable<CartDto>> ReadCartAsync();
        #endregion

        #region Order:
        void Insert(OrderDto model);
        void Update(OrderDto model);
        void Delete(OrderDto model);
        Task DeleteOrderAsync(int id);
        IEnumerable<OrderDto> ReadOrders();
        Task<IEnumerable<OrderDto>> ReadOrdersAsync();
        #endregion

        #region Product:
        void Insert(ProductDto model);
        void Update(ProductDto model);
        void Delete(ProductDto model);
        Task DeleteProductAsync(int id);
        IEnumerable<ProductDto> ReadProducts();
        Task<IEnumerable<ProductDto>> ReadProductsAsync();
        #endregion
    }
}
