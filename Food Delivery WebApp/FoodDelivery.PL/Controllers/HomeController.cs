namespace FoodDelivery.PL.Controllers
{
    using System;
    using Models;
    using PagedList;
    using System.Web;
    using System.Linq;
    using BLL.Services;
    using System.Web.Mvc;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using FoodDelivery.BLL.Dto;
    using Microsoft.AspNet.Identity.Owin;
    using FoodDelivery.BLL.Interfaces;

    public class HomeController : Controller
    {
        #region Fields:
        #region Heap:
        private readonly FoodDeliveryService fd;
        private ApplicationUser CurrentLocalUser { get; set; }
        private static CategoryDto Category { get; set; } = new CategoryDto();
        private string CurrentUserEmail { get; set; }

        #region Lists:
        private static List<CartDto> Carts { get; set; } = new List<CartDto>();
        private static List<ProductDto> Products { get; set; } = new List<ProductDto>();
        #endregion
        #endregion

        #region Stack:
        public static int CntAdd { get; set; } = 0;
        public static bool IsCart { get; set; } = false; 
        static bool isTopMenu = false;
        static bool isChangeQty = false;
        static bool IsCategoryPressed { get; set; } = false;
        #endregion
        #endregion

        #region SignInManager & UserManager:
        private ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }
        #endregion

        public HomeController()
        {
            fd = new FoodDeliveryService(Init.Connection);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> MainPage(int? page, string isShopCart)
        {
            if (bool.TryParse(isShopCart, out bool b))
                IsCart = true;

            #region ViewBags:
            ViewBag.TopMenu = isTopMenu;
            ViewBag.CategoryName = Category.Name;
            ViewBag.Categories = await fd.ReadCategoriesAsync();
            ViewBag.CategoryPressed = IsCategoryPressed;
            #endregion


            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(Products.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public async Task<ActionResult> MainPage(object page)
        {
            var button =
               Request.Params.Cast<string>().Where(p => p.StartsWith("btn")).Select(p => p.Substring("btn".Length)).First().Remove(0, 1);

            if (char.IsDigit(button[0])) // If we clicked to Add to cart
            {
                if (CurrentLocalUser == null)
                    return RedirectToAction("../Account/Register");
                var product = fd.ReadProducts().FirstOrDefault(f => f.Code == button);
                AddToCart(product);
            }
            else if (button != null) // If we Selected category
            {
                isTopMenu = false;
                IsCategoryPressed = true;
                Category = fd.ReadCategories().FirstOrDefault(n => n.TagName == button);
                RefreshList(new ProductDto(), true);
            }

            #region ViewBags:
            ViewBag.TopMenu = isTopMenu;
            ViewBag.CategoryName = Category.Name;
            ViewBag.CategoryPressed = IsCategoryPressed;
            ViewBag.Categories = await fd.ReadCategoriesAsync();
            #endregion

            int pageSize = 8;
            int pageNumber = (page as int? ?? 1);
            return View(Products.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public async Task<ActionResult> QtyChange(object[] carts, bool isQtyPress)
        {
            try
            {
                await Task.Run(() =>
                {
                    var numQty = carts.Take(carts.Length / 2).ToArray();
                    var itemIds = carts.Skip(carts.Length / 2).ToArray();
                    for (var i = 0; i < carts.Length / 2; i++)
                    {
                        var qty = int.Parse(numQty[i].ToString());
                        var crt = fd.ReadCart()
                            .FirstOrDefault(c => c.Id == int.Parse(itemIds[i].ToString()));
                        crt.Quantity = qty;
                        crt.ItemsPrice = crt.Price * qty;
                        fd.UpdateADO(crt, false);
                    }
                    isChangeQty = !isQtyPress ? isChangeQty = true : isChangeQty = false;
                });
            }
            catch { throw; }
            return RedirectToAction("MainPage", "Home", new { isShopCart = "cart" });
        }


        #region Auxiliary methods:
        private void AddToCart(ProductDto product)
        {
            fd.Insert(new CartDto
            {
                UserId = CurrentLocalUser.UserId,
                IsOrdered = false,
                ItemsPrice = product.Price,
                Price = product.Price,
                ProductName = product.Name,
                ProductPhoto = product.Photo,
                PutDate = DateTime.Now,
                Quantity = 1
            });
        }

        private void RefreshList(IModel model, bool isAuth = false)
        {
            if (isAuth || CurrentLocalUser != null)
            {
                switch (model.GetType().Name)
                {
                    case nameof(CartDto):
                        Carts = fd.ReadCart().Where(f => f.UserId == CurrentLocalUser.UserId)
                               .Where(o => o.IsOrdered == false).ToList();
                        break;
                    case nameof(ProductDto):
                        Products = Products = fd.ReadProducts().Where(i => i.CategoryId == Category.Id).ToList();
                        break;
                    case nameof(OrderDto):

                        break;
                    case nameof(MessageDto):

                        break;
                    default: break;
                }
            }
        }
        #endregion

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                CurrentUserEmail = filterContext.HttpContext.User.Identity.Name;
                try
                {
                    CurrentLocalUser = UserManager.Users.FirstOrDefault(u => u.Email == CurrentUserEmail);
                    RefreshList(new CartDto());
                }
                catch { }
            }
            else // When User Log off, or still doesn't enter 
            {
                CurrentUserEmail = null;
                CurrentLocalUser = null;
            }
        }
    }
}