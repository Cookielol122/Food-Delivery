namespace FoodDelivery.PL.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class fooddelivery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Last_Name", c => c.String());
            AddColumn("dbo.AspNetUsers", "Firs_Name", c => c.String());
            AddColumn("dbo.AspNetUsers", "Role", c => c.String());
            AddColumn("dbo.AspNetUsers", "DateOfRegister", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "UserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UserId");
            DropColumn("dbo.AspNetUsers", "DateOfRegister");
            DropColumn("dbo.AspNetUsers", "Role");
            DropColumn("dbo.AspNetUsers", "Firs_Name");
            DropColumn("dbo.AspNetUsers", "Last_Name");
        }
    }
}
