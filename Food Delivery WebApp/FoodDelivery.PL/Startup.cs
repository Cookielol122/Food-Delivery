using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FoodDelivery.PL.Startup))]
namespace FoodDelivery.PL
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
