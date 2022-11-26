namespace FoodDelivery.PL.Controllers
{
    using Models;
    using System.Linq;
    using BLL.Services;
    using System.Web.Mvc;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly FoodDeliveryService fd;

        public HomeController()
        {
            fd = new FoodDeliveryService(Init.Connection);
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> MainPage()
        {
            ViewBag.Categories = await fd.ReadCategoriesAsync();
            return View();
        }

        [HttpPost]
        public ActionResult MainPage(object o)
        {
            var button =
               Request.Params.Cast<string>().Where(p => p.StartsWith("btn")).Select(p => p.Substring("btn".Length)).First().Remove(0, 1);
            return View();
        }
    }
}