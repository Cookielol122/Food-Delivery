namespace FoodDelivery.PL.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.ApplicationDbContext>
    {    
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "FoodDelivery.PL.Models.ApplicationDbContext";
        }

        protected override void Seed(FoodDelivery.PL.Models.ApplicationDbContext context)
        {
        }
    }
}
