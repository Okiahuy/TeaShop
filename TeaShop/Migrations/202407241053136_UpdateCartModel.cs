namespace TeaShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCartModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Carts", "ImageURL");
            DropColumn("dbo.Carts", "ProductName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Carts", "ProductName", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.Carts", "ImageURL", c => c.String(maxLength: 255));
        }
    }
}
