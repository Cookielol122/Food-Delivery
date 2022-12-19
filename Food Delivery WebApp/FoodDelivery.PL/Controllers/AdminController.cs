using FoodDelivery.PL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using FoodDelivery.BLL.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FoodDelivery.BLL.Dto;
using System.Collections.Generic;

namespace FoodDelivery.PL.Controllers
{
    #region View Parts [enum]:
    public enum ViewPartType
    {
        UsersInfoPart,
        AllOrdersPart,
        NewPersonsPart,
        MessagesPart,
        ProductsPart,
        AddProductPart,
        EditProductPart,
        EditUserPart,
        MyDataPart,
        TotalIncomePart,
        AddCategoryPart,
        CategoryPart,
        EditCategoryPart,
        UsersLoginPass,
        NoN
    };
    #endregion

    public class AdminController : Controller
    {
        #region Fields:
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private static string ObjectType { get; set; }
        private static ApplicationUser CurrentAdmin { get; set; }
        private readonly FoodDeliveryService db;
        private static bool isError = false, _isPartView = false, isUserCredentials = false,
        isAddProduct = false, isNewMsg = false, isShowAllMsg = false;
        private static ProductDto SelectedProduct { get; set; }
        private static CategoryDto SelectedCategory { get; set; }
        private static string path;
        public static int CategoryId { get; set; }
        private static string PhotoPath { get; set; }
        #endregion

        #region SignInManager & UserManager:
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        public AdminController()
        {
            db = new FoodDeliveryService(Models.Init.Connection);
        }

        #region Login Admin:
        [AllowAnonymous]
        public ActionResult Admin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Admin(LoginViewModel admin, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsError = isError = true;
                if (admin.Email == null && admin.Password == null) ViewBag.Error = "Login and Password Fields have to be Filled out!";
                else ViewBag.Error = "Some of the fields are EMPTY!";
                return View(admin);
            }
            else
            {
                ApplicationUser signedUser = UserManager.FindByEmail(admin.Email);

                if (signedUser == null) // if login and password doesn't exists
                {
                    ViewBag.Error = "Your Login and/or Password is Incorrect!";
                    ViewBag.IsError = isError = true;
                    return View(admin);
                }

                SignInStatus result = await SignInManager.PasswordSignInAsync(signedUser.Email, admin.Password, admin.RememberMe, shouldLockout: false);

                switch (result)
                {
                    case SignInStatus.Success:
                        switch (signedUser.Role)
                        {
                            case "Admin":
                                CurrentAdmin = signedUser;
                                break;
                            default:
                                return View();
                        }
                        return RedirectToAction("_829528a_441d_484m_862i_22475963ffdn", new { token = CurrentAdmin.Id });
                    case SignInStatus.Failure:
                    default:
                        ViewBag.Error = "This Email or Password does not exist or the user is not confirmed!";
                        return View(admin);
                }
            }
        }
        #endregion

        #region Main Admin Page:
        public async Task<ActionResult> _829528a_441d_484m_862i_22475963ffdn(
           string token = null, bool isPartView = false, string partViewName = null,
           bool isShowMsg = false, string categId = null)
        {
            if (isShowMsg) isShowAllMsg = isShowMsg;
            if (isPartView)
            {
                _isPartView = isPartView;
                {
                    ViewBag.PartialView = partViewName;
                    if (partViewName == ViewPartType.UsersLoginPass.ToString())
                    { ViewBag.Credentials = GetUserCredential(true); }
                }
                ViewBag.IsMenuPart = isPartView;
                if (categId != null)
                {
                    isAddProduct = true;
                    var products = await db.ReadProductsAsync();
                    var prodBy = products.Where(p => p.CategoryId == int.Parse(categId)).ToList();
                    CategoryId = int.Parse(categId);
                    { ViewBag.Products = prodBy; }
                    { ViewBag.CategoryId = CategoryId; }
                    { ViewBag.Category = db.ReadCategories().FirstOrDefault(c => c.Id == CategoryId).Name; }
                }
                else
                {
                    ViewBag.Users = UserManager.Users.ToList();
                    ViewBag.CurrentUser = CurrentAdmin;
                }
            }
            if (Guid.TryParse(token, out Guid ui) || CurrentAdmin != null)
            {
                //var messages = await db.ReadMessagesAsync();
                //if (messages.Count != 0)
                //    isNewMsg = true;
                //else isNewMsg = false;
                { ViewBag.IsShownAllMsg = isShowAllMsg; }
                { ViewBag.IsNewMsg = isNewMsg; }
                //{ ViewBag.Messages = messages; }
                { ViewBag.Orders = await db.ReadOrdersAsync(); }
                { ViewBag.Users = UserManager.Users.ToList().OrderByDescending(d => d.DateOfRegister).ToList(); }
                try
                {
                    { ViewBag.AdminName = $"{CurrentAdmin.Firs_Name} {CurrentAdmin.Last_Name}"; }
                }
                catch { return RedirectToAction("Admin", "Admin"); }
                { ViewBag.Categories = await db.ReadCategoriesAsync(); }
                { ViewBag.IsPartialView = isPartView; }
                { ViewBag.IsStock = new SelectList(new string[] { "In Stock", "Out of Stock" }); }
                return View(UserManager.Users);
            }

            { ViewBag.IsShownAllMsg = isShowAllMsg; }
            { ViewBag.IsPartialView = isPartView; }
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<ActionResult> _829528a_441d_484m_862i_22475963ffdn(ProductDto product)
        {
            var button =
                Request.Params.Cast<string>().Where(p => p.StartsWith("btn")).Select(p => p.Substring("btn".Length)).First().Remove(0, 1);
            if (button != null)
            {
                var arr = button.Split(new char[] { '-' }); // 0 - action, 1 - Product Id
                switch (arr[0])
                {
                    case "AddProduct":
                        ObjectType = "Product";
                        ViewBag.PartialView = ViewPartType.AddProductPart.ToString();
                        ViewBag.IsMenuPart = _isPartView = true;
                        break;
                    case "AddCategory":
                        ObjectType = "Category";
                        ViewBag.PartialView = ViewPartType.AddCategoryPart.ToString();
                        ViewBag.IsMenuPart = _isPartView = true;
                        break;
                    case "AddCategoryToDB":
                        var photo = PhotoPath ==
                           null || PhotoPath == "" ? "" : PhotoPath;
                        var tag = string.Join("", product.Name.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                        var categ = new CategoryDto
                        {
                            Name = product.Name,
                            Photo = photo,
                            TagName = tag
                        };
                        db.Insert(categ); // Put the new category to datgabase
                        return RedirectToAction("_829528a_441d_484m_862i_22475963ffdn", "Admin", new { isPartView = true, partViewName = "CategoryPart" });
                    case "AddProductToDB":
                        product.DateAdded = DateTime.Now;
                        product.CategoryId = CategoryId;
                        product.Photo = PhotoPath == null ? "" : PhotoPath;
                        db.Insert(product); // Put the new product to datgabase
                        return RedirectToAction("_829528a_441d_484m_862i_22475963ffdn", "Admin", new { isPartView = true, partViewName = "ProductsPart", categId = CategoryId, });
                    case "Edit":
                        ObjectType = "Product";
                        ViewBag.SelectedProduct = SelectedProduct = db.ReadProducts().FirstOrDefault(f => f.Id == int.Parse(arr[1]));
                        ViewBag.PartialView = ViewPartType.EditProductPart.ToString();
                        ViewBag.IsMenuPart = _isPartView = true;
                        break;
                    case "EditCateg":
                        ObjectType = "Category";
                        ViewBag.SelectedCategory
                            = SelectedCategory = db.ReadCategories().FirstOrDefault(f => f.Id == int.Parse(arr[1]));
                        ViewBag.PartialView = ViewPartType.EditCategoryPart.ToString();
                        ViewBag.IsMenuPart = _isPartView = true;
                        break;
                    case "EditCategory":
                        var phto = PhotoPath ==
                            null || PhotoPath == "" ? SelectedCategory.Photo : PhotoPath;
                        var c = new CategoryDto
                        {
                            Id = SelectedCategory.Id,
                            Name = product.Name,
                            Photo = phto,
                            TagName = SelectedCategory.TagName
                        };
                        db.Update(c);
                        return RedirectToAction("_829528a_441d_484m_862i_22475963ffdn", "Admin", new { isPartView = true, partViewName = "CategoryPart" });
                    case "EditProduct":
                        product.Id = SelectedProduct.Id;
                        product.DateAdded = DateTime.Now;
                        product.CategoryId = CategoryId;
                        product.Photo = PhotoPath == null || PhotoPath == "" ? SelectedProduct.Photo : PhotoPath;
                        db.Update(product);
                        return RedirectToAction("_829528a_441d_484m_862i_22475963ffdn", "Admin", new { isPartView = true, partViewName = "ProductsPart", categId = CategoryId });
                    case "Details":
                        break;
                    case "Delete":
                        if (int.TryParse(arr[1], out int id))
                        {
                            var prod = db.ReadProducts().FirstOrDefault(i => i.Id == int.Parse(arr[1]));
                            await db.DeleteProductAsync(int.Parse(arr[1]));
                            try
                            {
                                string path = Path.Combine(Server.MapPath($"~/Photos/Products/{prod.Photo}"));
                                System.IO.File.Delete(path);
                            }
                            catch { }
                        }
                        { ViewBag.IsShownAllMsg = isShowAllMsg; }
                        ViewBag.IsMenuPart = _isPartView = true;
                        return RedirectToAction("_829528a_441d_484m_862i_22475963ffdn", "Admin", new { isPartView = true, partViewName = "ProductsPart", categId = CategoryId, });
                    case "DeleteCateg":
                        if (int.TryParse(arr[1], out int Id))
                        {
                            var categ1 = db.ReadCategories().FirstOrDefault(i => i.Id == int.Parse(arr[1]));
                            try
                            {
                                await db.DeleteCategoryAsync(int.Parse(arr[1]));
                                string path = Path.Combine(Server.MapPath($"~/Photos/Categories/{categ1.Photo}"));
                                System.IO.File.Delete(path);
                            }
                            catch { }
                        }
                        { ViewBag.IsShownAllMsg = isShowAllMsg; }
                        ViewBag.IsMenuPart = _isPartView = true;
                        return RedirectToAction("_829528a_441d_484m_862i_22475963ffdn", "Admin", new { isPartView = true, partViewName = "CategoryPart" });
                }
            }
            { ViewBag.IsShownAllMsg = isShowAllMsg; }
            { ViewBag.IsNewMsg = isNewMsg; }
            //{ ViewBag.Messages = await db.ReadMessagesAsync(); }
            { ViewBag.Orders = await db.ReadOrdersAsync(); }
            { ViewBag.Users = UserManager.Users.ToList(); }
            { ViewBag.CategoryId = CategoryId; }
            { ViewBag.Users = UserManager.Users.ToList(); }
            { ViewBag.AdminName = $"{CurrentAdmin.Firs_Name} {CurrentAdmin.Last_Name}"; }
            { ViewBag.Categories = await db.ReadCategoriesAsync(); }
            { ViewBag.IsPartialView = _isPartView; }
            return View(UserManager.Users);
        }
        #endregion



        #region Auxiliary methods:
        private IEnumerable<Credential> GetUserCredential(bool isOk, string pathName = null)
        {
            string[] fromFile = default, eachLineSplt = default;
            var list = new List<Credential>();
            if (isOk && pathName == null)
            {
                fromFile =
                    System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "StoragePass.txt");
                eachLineSplt = new string[6]; var cnt = 0;
                for (var i = 0; i < fromFile.Length; i++)
                {
                    eachLineSplt = fromFile[i].Split(new char[] { ' ' });
                    var cred = new Credential();
                    cred.RecordDate = Convert.ToDateTime(eachLineSplt[0] + " " + eachLineSplt[1]);
                    cred.Email = eachLineSplt[4];
                    cred.Password = eachLineSplt[6];
                    cred.Id = ++cnt;
                    list.Add(cred);
                }
            }
            return list.OrderByDescending(d => d.RecordDate).ToList();
        }

        private object GetPartialView(ViewPartType vPart)
        {
            return new object();
        }
        #endregion
    }
}