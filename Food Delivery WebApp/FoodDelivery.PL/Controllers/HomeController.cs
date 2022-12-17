namespace FoodDelivery.PL.Controllers
{
    using System;
    using Models;
    using PagedList;
    using System.Web;
    using System.Linq;
    using BLL.Services;
    using System.Web.Mvc;
    using FoodDelivery.BLL.Dto;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using FoodDelivery.BLL.Interfaces;
    using Microsoft.AspNet.Identity.Owin;
    using FoodDelivery.BLL.Dto.EmailModel;
    using System.IO;

    public class HomeController : Controller
    {
        #region Fields:
        #region Heap:
        private readonly FoodDeliveryService fd;
        private ApplicationUser CurrentLocalUser { get; set; }
        private static CategoryDto Category { get; set; } = new CategoryDto();
        private string SelectedProdName { get; set; }
        private string CurrentUserEmail { get; set; }
        public static object ModalMsg { get; set; }

        #region Lists:
        private static List<CartDto> Carts { get; set; } = new List<CartDto>();
        private static List<ProductDto> Products { get; set; } = new List<ProductDto>();
        #endregion
        #endregion
        #region Stack:
        public static int CntAdd { get; set; } = 0;
        public static bool IsOrder { get; set; } = false;
        public static bool IsCart { get; set; } = false;
        public static bool IsModal { get; set; }
        public static int TotalSum { get; set; }
        public static bool isChangeQty = false, IsEmptyCart = false;
        public static bool isTopMenu = false;
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
        public async Task<ActionResult> MainPage(int? page, string param = "AllCategoryPart", string isShopCart = "")
        {
            if (bool.TryParse(isShopCart, out bool b))
            {
                IsOrder = true;
                IsCart = true;
                FillCart();
            }
            if (page != null) param = null;
            if (param != null)
                ViewBag.PartialView = TopMenuClicked(param);
            else isTopMenu = false;

            #region ViewBags:
            ViewBag.Cart = Carts;
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
            var res = ClickAction(button);
            if(res == null) return RedirectToAction("../Account/Register");
            else if (res.ToString() == "Success")
                ModalMsg = LoggedIn();

            #region ViewBags:
            ViewBag.TopMenu = isTopMenu;
            ViewBag.Cart = Carts;
            ViewBag.CategoryName = Category.Name;
            ViewBag.CategoryPressed = IsCategoryPressed;
            ViewBag.Categories = await fd.ReadCategoriesAsync();
            #endregion

            int pageSize = 8;
            int pageNumber = (page as int? ?? 1);
            return View(Products.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CloseAlerts()
        {
            IsModal = false;
            return RedirectToAction("../Home/MainPage");
        }

        [HttpGet]
        public async Task<ActionResult> DeleteItem(int? cartSelected)
        {
            if (cartSelected != null)
            {
                --CntAdd;
                var cart = Carts.FirstOrDefault(i => i.Id == cartSelected);
                TotalSum -= cart.ItemsPrice;
                var id = int.Parse(cartSelected.ToString());
                await fd.DeleteCartAsync(id);
                Carts.Remove(cart);
            }
            return RedirectToAction("MainPage", "Home", new { isShopCart = "true" }); // ..on 3rd params pass 'true' to left cart deployed
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
                        var resMsg = fd.UpdateADO(crt, false);
                    }
                    isChangeQty = isQtyPress ? isChangeQty = true : isChangeQty = false;
                });
            }
            catch { throw; }
            return RedirectToAction("MainPage", "Home", new { isShopCart = "cart" });
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage(EmailData mailInfo)
        {
            IsModal = true;
            mailInfo.SendingDate = DateTime.UtcNow;
            if (mailInfo.Message.Equals("MakeOrder"))
            {
                TotalSum = 0;
                var arr = MakeOrderFile(mailInfo.UserName, mailInfo.Phone, mailInfo.Email);
                var carts = Carts.Where(u => u.UserId == CurrentLocalUser.UserId).ToList();
                var productsStr = string.Empty;
                foreach (var pr in carts)
                    productsStr += pr.ProductName + ", ";
                var htmlB = CreateOrderBody(carts, mailInfo);
                fd.Insert(new OrderDto
                {
                    OrderDate = DateTime.Now,
                    TotalAmount = carts.Sum(t => t.ItemsPrice),
                    UserId = CurrentLocalUser.UserId,
                    OrderProducts = productsStr
                });
                ModalMsg = new EmailService(mailInfo, htmlB)
                    .SendMessage(MessageType.NewOrder, arr[0]/*, "andriygerbut@gmail.com"*/);
                foreach (var item in carts)
                {
                    item.IsOrdered = true;
                    fd.UpdateADO(item);
                }
                return RedirectToAction("../Home/MainPage");
            }
            else
            {
                await fd.Insert(new MessageDto
                {
                    TypeMessage = "Write Us",
                    TextMessage = mailInfo.Comment,
                    DateMessage = DateTime.Now,
                    IsReviewed = false,
                    RecipientId = 1,
                    //SenderId = CurrentLocalUser.UserId,
                    Title = $"'{mailInfo.Message}' from foodspace.com"
                });
                ModalMsg = new EmailService(mailInfo).SendMessage(MessageType.WriteUs);
            }
            return RedirectToAction("../Home/MainPage");
        }

        private object LoggedIn()
        {
            if (CurrentLocalUser == null)
                return RedirectToAction("../Account/Register");
            else return $"'{SelectedProdName}' додано в корзину!";
        }

        #region Auxiliary methods:
        /// <summary>
        /// Метод, який створює текстовий файл, для прикріплення
        /// </summary>
        /// <param name="order"> Список замовлень </param>
        /// <param name="userName"> Імя клієнта </param>
        /// <returns> 1st - шлях до файлу, 2nd - Успіх </returns>
        private string[] MakeOrderFile(string userName, string phone, string email, bool isCreateFile = true)
        {
            var returnArr = new string[2];
            var randNum = new Random();
            var dateFile = DateTime.UtcNow;
            returnArr[0] = AppDomain.CurrentDomain.BaseDirectory +
                $"OrdersFiles\\Order - {dateFile.Day + "." + dateFile.Month + "." + dateFile.Year + "  " + dateFile.Hour + "." + dateFile.Minute + "." + dateFile.Second + "." + dateFile.Millisecond}.txt";
            var carts = Carts.Where(u => u.UserId == CurrentLocalUser.UserId).ToList();
            if (isCreateFile)
            {
                using (var sw = new StreamWriter(returnArr[0]))
                {
                    if (System.IO.File.Exists(returnArr[0]))
                    {
                        sw.WriteLine(new string('-', 100));
                        sw.WriteLine($"Замовлення № '{Guid.NewGuid()}':\n{DateTime.UtcNow}");
                    }
                    sw.WriteLine(new string('-', 100) + "\n");
                    sw.WriteLine($"Привіт, я {userName}\nЯ зробив наступне замовлення:\n");
                    for (var i = 0; i < carts.Count; i++)
                    {
                        sw.WriteLine($"{i + 1}.");
                        sw.WriteLine($"Назва продукту:\t{carts[i].ProductName}");
                        sw.WriteLine($"К-сть:\t{carts[i].Quantity}");
                        sw.WriteLine($"Ціна:\t{carts[i].ItemsPrice}");
                        sw.WriteLine($"Дата замовлення:\t{carts[i].PutDate}");
                        sw.WriteLine(new string('-', 100) + "\n");
                        //TotalSum = carts[i].ItemsPrice;
                    }
                    sw.WriteLine($"\n\n\t\t\t\t\t\t\t\t\t\tДо сплати: {carts.Sum(a => a.ItemsPrice)}грн.");
                    sw.WriteLine("---------------------------------------------");
                    sw.WriteLine("Мої контакти:");
                    sw.WriteLine($"E-mail:\t{email}\nТелефон:\t{phone}");
                    sw.WriteLine("---------------------------------------------");
                    sw.WriteLine(new string('-', 100));
                    sw.WriteLine(new string('-', 100) + "\n\n");
                    returnArr[1] = "Успіх! Інформація про замовлення надіслана до нас.";
                }
            }
            else returnArr[1] = "Успіх! Інформація про замовлення надіслана до нас";
            return returnArr;
        }

        private static string htmlBody = string.Empty;
        private string CreateOrderBody(List<CartDto> carts, EmailData data)
        {
            string subHtml = string.Empty;
            foreach (var c in carts)
            {
                subHtml +=
                    "<tr style=\"text-align:center\">" +
                     $"<td scope = \"row\"><a href = \"#\">{c.ProductName}</a></td>" +
                     $"<td scope = \"row\">{c.Quantity}</td> " +
                     $"<td scope = \"row\">{c.Price}</td> " +
                     $"<td scope = \"row\">{c.ItemsPrice}</td> " +
                    "</tr> ";
            }
            htmlBody =
"<!DOCTYPE html>" +
"<html>" +
"<head>" +
   "<meta charset = \"utf-8\" /> " +
    "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
    "<link href = \"https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css\" rel=\"stylesheet\">" +
"</head>" +
"<body>" +
    $"<h3 style = \"text-align:center;color:blue\" > Order № {carts.Count} {DateTime.Now}</h3>" +
    "<h5>Привіт, Foodspace!</h5>" +
    "<h5>" +
        "Yjdt pfvjdktyyz p &nbsp;&nbsp;<a href = \"http://www.foodspace.com\">" +
            "foodspace.com" +
        "</a> " +
    "</h5> " +
    "<h1 style=\"text-align:center\">Інформація про замовлення</h1>" +
    "<table style=\"padding:3px;text-align:center\" class=\"table table-bordered\">" +
        "<tr style = \"text-align:center;padding:3px;background-color:whitesmoke\"> " +
           " <th style=\"padding:5px\" scope=\"col\">Назва продукту</th>" +
           " <th style=\"padding:5px\" scope = \"col\" > К-сть </ th > " +
           " <th style=\"padding:5px\" scope=\"col\">Ціна</th>" +
           " <th style=\"padding:5px\" scope = \"col\" > Загальна ціна</th>" +
       " </tr>" +
      subHtml +
    "</table>" +
    "<div style = \"border: thin solid sandybrown; background-color: sandybrown;padding:8px\"> " +
        $"<h2 style=\"text-align:right\">До сплати:&nbsp;&nbsp;{carts.Sum(a => a.ItemsPrice).ToString("N2")} ГРН</h2>" +
    "</div>" +
    "<div>" +
        "<h3 style = \"color:orange\"> Деталі оплати</h3>" +
        "<table style=\"padding:3px\" class=\"table table-bordered\">" +
            "<tr style=\"padding:3px\">" +
                "<th style = \"background-color:silver;padding:3px\" > Метод оплати</th>" +
                "<th style = \"background-color:orange;padding:3px\">Payment in cash</th>" +
            "</tr>" +
            "<tr style=\"padding:3px\">" +
                "<th style = \"background-color:silver;padding:3px\" > Статус оплати</th>" +
                "<th style = \"background-color:orange;padding:3px\">Unpaid</th>" +
            "</tr>" +
      "  </table>" +
       " <h3 style = \"color:orange\"> Customer's info</h3>" +
        "<table style=\"padding:3px\" class=\"table table-bordered\">" +
            "<tr style=\"padding:3px\">" +
               " <th style = \"background-color:silver;padding:3px\" > І'мя </th>" +
               $"<th style = \"background-color:yellow;padding:3px\">{data.UserName}</th>" +
           " </tr>" +
            "<tr style=\"padding:3px\">" +
                "<th style = \"background-color:silver;padding:3px\"> Телефон </th>" +
               $"<th style = \"background-color:yellow;padding:3px\">{data.Phone}</th>" +
           " </tr>" +
            "<tr style=\"padding:3px\">" +
                "<th style = \"background-color:silver;padding:3px\" > Адреса доставки, № відділення</th>" +
               $"<th style = \"background-color:yellow;padding:3px\">{data.Comment}</th>" +
            "</tr>  <tr style=\"padding:3px\">" +
               " <th style = \"background-color:silver;padding:3px\"> E-Mail </th>" +
               $"<th style = \"background-color:yellow;padding:3px\">{data.Email}</th>" +
               " </tr>" +
       " </table></div></body></html>";

            return htmlBody;
        }

        private object ClickAction(string button)
        {
            if (char.IsDigit(button[0])) // If we clicked to Add to cart
                return AddToCart(fd.ReadProducts().FirstOrDefault(f => f.Code == button));
            else if (button != null)     // If we Selected category
            {
                isTopMenu = false;
                IsCategoryPressed = true;
                Category = fd.ReadCategories().FirstOrDefault(n => n.TagName == button);
                RefreshList(new ProductDto(), true);
                return $"Вибрана категорія - '{button}'";
            }
            else return new object();
        }

        private object AddToCart(ProductDto product)
        {
            try
            {
                if (CurrentLocalUser != null)
                {
                    SelectedProdName = product.Name;
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
                    return "Success";
                }
                else return null;
            }
            catch (Exception ex) { return ex.Message; }
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

        private void FillCart()
        {
            if (CurrentLocalUser != null)
            {
                Carts = fd.ReadCart()
                    .Where(u => u.UserId == CurrentLocalUser.UserId)
                    .Where(i => i.IsOrdered == false).ToList();
                TotalSum = Carts.Sum(t => t.ItemsPrice); // View cart's total sum 
                CntAdd = Carts.Count;
                if (Carts.Count() == 0)
                    IsEmptyCart = true;
            }
        }

        private object TopMenuClicked(string param)
        {
            isTopMenu = true;
            switch (param)
            {
                case "Contacts":
                    return "ContactPart";
                case "Delivery":
                    return "DeliveryPart";
                case "Payments":
                    return "PaymentsPart";
                case "AllCategoryPart":
                    return "AllCategoryPart";
                default:
                    return "NoN Partial";
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