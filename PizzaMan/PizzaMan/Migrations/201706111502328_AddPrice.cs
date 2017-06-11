namespace PizzaMan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pizzas", "Price", c => c.Double(nullable: false));
            DropColumn("dbo.Pizzas", "Details");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pizzas", "Details", c => c.String());
            DropColumn("dbo.Pizzas", "Price");
        }
    }
}
