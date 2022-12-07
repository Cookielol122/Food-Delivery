namespace FoodDelivery.BLL.Services
{
    using System;
    using DAL.UOF;
    using System.Linq;
    using DAL.Entities;
    using DAL.Interfaces;
    using FoodDelivery.BLL.Dto;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class FoodDeliveryService : Interfaces.IFoodDeliveryService
    {
        private readonly IUnitOfWork Db;
        private readonly string connectStrCpy;

        public FoodDeliveryService(string connectStr)
        {
            connectStrCpy = connectStr;
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
                Id = dto.Id,
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
            var dto = new List<CategoryDto>();
            var list = Db.Categories.GetAll().ToList();
            for (int i = 0; i < list.Count; i++)
            {
                dto.Add(new CategoryDto());
                dto[i].Id = list[i].Id;
                dto[i].Name = list[i].Name;
                dto[i].TagName = list[i].TagName;
                dto[i].Photo = list[i].Photo;
            }
            return dto;
        }

        public async Task<IEnumerable<CategoryDto>> ReadCategoriesAsync()
        {
            var dto = new List<CategoryDto>();
            var list = await Db.Categories.GetAllAsync();
            for (int i = 0; i < list.Count; i++)
            {
                dto.Add(new CategoryDto());
                dto[i].Id = list[i].Id;
                dto[i].Name = list[i].Name;
                dto[i].TagName = list[i].TagName;
                dto[i].Photo = list[i].Photo;
            }
            return dto;
        }
        #endregion

        #region Cart service:
        public void Insert(CartDto dto)
        {
            var cart = new ShoppingCart
            {
                IsOrdered = dto.IsOrdered,
                ItemsPrice = dto.ItemsPrice,
                Price = dto.Price,
                ProductName = dto.ProductName,
                ProductPhoto = dto.ProductPhoto,
                PutDate = dto.PutDate,
                Quantity = dto.Quantity,
                UserId = dto.UserId
            };
            Db.ShoppingCarts.Create(cart);
        }

        public void Update(CartDto dto)
        {
            var cart = new ShoppingCart
            {
                Id = dto.Id,
                IsOrdered = dto.IsOrdered,
                ItemsPrice = dto.ItemsPrice,
                Price = dto.Price,
                ProductName = dto.ProductName,
                ProductPhoto = dto.ProductPhoto,
                PutDate = dto.PutDate,
                Quantity = dto.Quantity,
                UserId = dto.UserId
            };
            Db.ShoppingCarts.Update(cart);
        }

        public void Delete(CartDto dto)
        {
            var cart = new ShoppingCart
            {
                Id = dto.Id,
                IsOrdered = dto.IsOrdered,
                ItemsPrice = dto.ItemsPrice,
                Price = dto.Price,
                ProductName = dto.ProductName,
                ProductPhoto = dto.ProductPhoto,
                PutDate = dto.PutDate,
                Quantity = dto.Quantity,
                UserId = dto.UserId
            };
            Db.ShoppingCarts.Delete(cart);
        }

        public async Task DeleteCartAsync(int id)
        {
            await Db.ShoppingCarts.DeleteAsync(id);
        }

        public IEnumerable<CartDto> ReadCart()
        {
            var dto = new List<CartDto>();
            var list = Db.ShoppingCarts.GetAll().ToList();
            for (int i = 0; i < list.Count; i++)
            {
                dto.Add(new CartDto());
                dto[i].Id = list[i].Id;
                dto[i].IsOrdered = list[i].IsOrdered;
                dto[i].ItemsPrice = list[i].ItemsPrice;
                dto[i].Price = list[i].Price;
                dto[i].ProductName = list[i].ProductName;
                dto[i].ProductPhoto = list[i].ProductPhoto;
                dto[i].PutDate = list[i].PutDate;
                dto[i].Quantity = list[i].Quantity;
                dto[i].UserId = list[i].UserId;
            }
            return dto;
        }

        public async Task<IEnumerable<CartDto>> ReadCartAsync()
        {
            var dto = new List<CartDto>();
            var list = await Db.ShoppingCarts.GetAllAsync();
            for (int i = 0; i < list.Count; i++)
            {
                dto.Add(new CartDto());
                dto[i].Id = list[i].Id;
                dto[i].IsOrdered = list[i].IsOrdered;
                dto[i].ItemsPrice = list[i].ItemsPrice;
                dto[i].Price = list[i].Price;
                dto[i].ProductName = list[i].ProductName;
                dto[i].ProductPhoto = list[i].ProductPhoto;
                dto[i].PutDate = list[i].PutDate;
                dto[i].Quantity = list[i].Quantity;
                dto[i].UserId = list[i].UserId;
            }
            return dto;
        }

        public string UpdateADO(CartDto cart, bool isOrdered = true)
        {
            if (isOrdered)
            {
                string qwery = $"update ShoppingCarts set IsOrdered=@IsOrdered where Id={cart.Id}";
                using (var conn = new SqlConnection(connectStrCpy))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(qwery, conn))
                    {
                        cmd.Parameters.AddWithValue("@IsOrdered", cart.IsOrdered);
                        try
                        {
                            var res = cmd.ExecuteNonQuery();
                            return res == 1 ? "Update Success!" : "Something went wrong...";
                        }
                        catch (Exception ex) { return ex.Message; }
                    }
                }
            }
            else
            {
                string qwery = $"update ShoppingCarts set ItemsPrice=@ItemsPrice, Quantity=@Quantity where Id={cart.Id}";
                using (var conn = new SqlConnection(connectStrCpy))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(qwery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ItemsPrice", cart.ItemsPrice);
                        cmd.Parameters.AddWithValue("@Quantity", cart.Quantity);
                        try
                        {
                            var res = cmd.ExecuteNonQuery();
                            return res == 1 ? "Update Success!" : "Something went wrong...";
                        }
                        catch (Exception ex) { return ex.Message; }
                    }
                }
            }
        }
        #endregion

        #region Order service:
        public void Insert(OrderDto dto)
        {
            var order = new Order
            {
                OrderDate = dto.OrderDate,
                OrderProducts = dto.OrderProducts,
                TotalAmount = dto.TotalAmount
            };
            Db.Orders.Create(order);
        }

        public void Update(OrderDto dto)
        {
            var order = new Order
            {
                Id = dto.Id,
                OrderDate = dto.OrderDate,
                OrderProducts = dto.OrderProducts,
                TotalAmount = dto.TotalAmount
            };
            Db.Orders.Update(order);
        }

        public void Delete(OrderDto dto)
        {
            var order = new Order
            {
                Id = dto.Id,
                OrderDate = dto.OrderDate,
                OrderProducts = dto.OrderProducts,
                TotalAmount = dto.TotalAmount
            };
            Db.Orders.Delete(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            await Db.Orders.DeleteAsync(id);
        }

        public IEnumerable<OrderDto> ReadOrders()
        {
            var dto = new List<OrderDto>();
            var list = Db.Orders.GetAll().ToList();
            for (int i = 0; i < list.Count; i++)
            {
                dto.Add(new OrderDto());
                dto[i].Id = list[i].Id;
                dto[i].OrderDate = list[i].OrderDate;
                dto[i].OrderProducts = list[i].OrderProducts;
                dto[i].TotalAmount = list[i].TotalAmount;
                dto[i].UserId = list[i].UserId;
            }
            return dto;
        }

        public async Task<IEnumerable<OrderDto>> ReadOrdersAsync()
        {
            var dto = new List<OrderDto>();
            var list = await Db.Orders.GetAllAsync();
            for (int i = 0; i < list.Count; i++)
            {
                dto.Add(new OrderDto());
                dto[i].Id = list[i].Id;
                dto[i].OrderDate = list[i].OrderDate;
                dto[i].OrderProducts = list[i].OrderProducts;
                dto[i].TotalAmount = list[i].TotalAmount;
                dto[i].UserId = list[i].UserId;
            }
            return dto;
        }
        #endregion

        #region Product service:
        public void Insert(ProductDto dto)
        {
            var product = new Product
            {
                CategoryId = dto.CategoryId,
                Code = dto.Code,
                DateAdded = dto.DateAdded,
                Description = dto.Description,
                IsPromotion = dto.IsPromotion,
                isStock = dto.isStock,
                Name = dto.Name,
                Photo = dto.Photo,
                Price = dto.Price
            };
            Db.Products.Create(product);
        }

        public void Update(ProductDto dto)
        {
            var product = new Product
            {
                Id = dto.Id,
                CategoryId = dto.CategoryId,
                Code = dto.Code,
                DateAdded = dto.DateAdded,
                Description = dto.Description,
                IsPromotion = dto.IsPromotion,
                isStock = dto.isStock,
                Name = dto.Name,
                Photo = dto.Photo,
                Price = dto.Price
            };
            Db.Products.Update(product);
        }
        public void Delete(ProductDto dto)
        {
            var product = new Product
            {
                Id = dto.Id,
                CategoryId = dto.CategoryId,
                Code = dto.Code,
                DateAdded = dto.DateAdded,
                Description = dto.Description,
                IsPromotion = dto.IsPromotion,
                isStock = dto.isStock,
                Name = dto.Name,
                Photo = dto.Photo,
                Price = dto.Price
            };
            Db.Products.Delete(product);
        }
        public async Task DeleteProductAsync(int id)
        {
            await Db.Products.DeleteAsync(id);
        }

        public IEnumerable<ProductDto> ReadProducts()
        {
            var dto = new List<ProductDto>();
            var list = Db.Products.GetAll().ToList();
            for (int i = 0; i < list.Count; i++)
            {
                dto.Add(new ProductDto());
                dto[i].Id = list[i].Id;
                dto[i].Description = list[i].Description;
                dto[i].CategoryId = list[i].CategoryId;
                dto[i].Code = list[i].Code;
                dto[i].DateAdded = list[i].DateAdded;
                dto[i].IsPromotion = list[i].IsPromotion;
                dto[i].isStock = list[i].isStock;
                dto[i].Name = list[i].Name;
                dto[i].Photo = list[i].Photo;
                dto[i].Price = list[i].Price;
            }
            return dto;
        }

        public async Task<IEnumerable<ProductDto>> ReadProductsAsync()
        {
            var dto = new List<ProductDto>();
            var list = await Db.Products.GetAllAsync();
            for (int i = 0; i < list.Count; i++)
            {
                dto.Add(new ProductDto());
                dto[i].Id = list[i].Id;
                dto[i].Description = list[i].Description;
                dto[i].CategoryId = list[i].CategoryId;
                dto[i].Code = list[i].Code;
                dto[i].DateAdded = list[i].DateAdded;
                dto[i].IsPromotion = list[i].IsPromotion;
                dto[i].isStock = list[i].isStock;
                dto[i].Name = list[i].Name;
                dto[i].Photo = list[i].Photo;
                dto[i].Price = list[i].Price;
            }
            return dto;
        }
        #endregion

        #region Message service:
        public async Task Insert(MessageDto dto)
        {
            await Task.Run(() =>
            {
                var message = new Message
                {
                    DateMessage = dto.DateMessage,
                    IsReviewed = dto.IsReviewed,
                    RecipientId = dto.RecipientId,
                    SenderId = dto.SenderId,
                    TextMessage = dto.TextMessage,
                    Title = dto.Title,
                    TypeMessage = dto.TypeMessage
                };
                Db.Messages.Create(message);
            });
        }
        #endregion
    }
}
