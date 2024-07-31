namespace TeaShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migrations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartDetails",
                c => new
                    {
                        CartDetailID = c.Int(nullable: false, identity: true),
                        CartID = c.Int(nullable: false),
                        ProductID = c.String(nullable: false, maxLength: 255),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Size = c.String(nullable: false, maxLength: 50),
                        Image = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.CartDetailID)
                .ForeignKey("dbo.Carts", t => t.CartID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.CartID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        CartID = c.Int(nullable: false, identity: true),
                        CustomerID = c.String(nullable: false, maxLength: 255),
                        CreatedDate = c.DateTime(nullable: false),
                        ImageURL = c.String(maxLength: 255),
                        ProductName = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.CartID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerID = c.String(nullable: false, maxLength: 255),
                        CustomerName = c.String(nullable: false, maxLength: 255),
                        Phone = c.String(maxLength: 20),
                        Email = c.String(maxLength: 255),
                        Address = c.String(maxLength: 255),
                        Username = c.String(nullable: false, maxLength: 255),
                        PasswordHash = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.CustomerID);
            
            CreateTable(
                "dbo.CustomerReviews",
                c => new
                    {
                        ReviewID = c.Int(nullable: false, identity: true),
                        CustomerID = c.String(nullable: false, maxLength: 255),
                        ProductID = c.String(nullable: false, maxLength: 255),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(),
                        ReviewDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.CustomerID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.String(nullable: false, maxLength: 255),
                        ProductName = c.String(nullable: false, maxLength: 255),
                        CategoryID = c.String(maxLength: 255),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Stock = c.Int(nullable: false),
                        Description = c.String(),
                        ImageURL = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Categories", t => t.CategoryID)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.String(nullable: false, maxLength: 255),
                        CategoryName = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderDetailID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        ProductID = c.String(nullable: false, maxLength: 255),
                        Quantity = c.Int(nullable: false),
                        Size = c.String(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.OrderDetailID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        CustomerID = c.String(nullable: false, maxLength: 255),
                        OrderDate = c.DateTime(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderStatus = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.EmployeeSales",
                c => new
                    {
                        SaleID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(nullable: false),
                        OrderID = c.Int(nullable: false),
                        SaleDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SaleID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.EmployeeID)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false, identity: true),
                        EmployeeName = c.String(nullable: false, maxLength: 255),
                        PositionID = c.String(maxLength: 255),
                        Phone = c.String(maxLength: 20),
                        Email = c.String(maxLength: 255),
                        Address = c.String(maxLength: 255),
                        HireDate = c.DateTime(nullable: false),
                        Username = c.String(nullable: false, maxLength: 255),
                        Password = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.Positions", t => t.PositionID)
                .Index(t => t.PositionID);
            
            CreateTable(
                "dbo.EmployeeSalarys",
                c => new
                    {
                        SalaryID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        TotalSales = c.Int(nullable: false),
                        SalaryAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.SalaryID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.Positions",
                c => new
                    {
                        PositionID = c.String(nullable: false, maxLength: 255),
                        PositionName = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.PositionID);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        PaymentMethodID = c.Int(nullable: false),
                        PaymentDate = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PaymentID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.PaymentMethods", t => t.PaymentMethodID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.PaymentMethodID);
            
            CreateTable(
                "dbo.PaymentMethods",
                c => new
                    {
                        PaymentMethodID = c.Int(nullable: false, identity: true),
                        MethodName = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.PaymentMethodID);
            
            CreateTable(
                "dbo.PromotionDetails",
                c => new
                    {
                        PromotionDetailID = c.Int(nullable: false, identity: true),
                        PromotionID = c.String(nullable: false, maxLength: 255),
                        ProductID = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.PromotionDetailID)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .ForeignKey("dbo.Promotions", t => t.PromotionID, cascadeDelete: true)
                .Index(t => t.PromotionID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Promotions",
                c => new
                    {
                        PromotionID = c.String(nullable: false, maxLength: 255),
                        PromotionName = c.String(nullable: false, maxLength: 255),
                        DiscountPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PromotionID);
            
            CreateTable(
                "dbo.Recipis",
                c => new
                    {
                        RecipeID = c.Int(nullable: false, identity: true),
                        ProductID = c.String(nullable: false, maxLength: 255),
                        IngredientID = c.String(nullable: false, maxLength: 255),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.RecipeID)
                .ForeignKey("dbo.Ingredients", t => t.IngredientID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID)
                .Index(t => t.IngredientID);
            
            CreateTable(
                "dbo.Ingredients",
                c => new
                    {
                        IngredientID = c.String(nullable: false, maxLength: 255),
                        IngredientName = c.String(nullable: false, maxLength: 255),
                        SupplierID = c.String(maxLength: 255),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IngredientID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID)
                .Index(t => t.SupplierID);
            
            CreateTable(
                "dbo.IngredientStocks",
                c => new
                    {
                        StockID = c.Int(nullable: false, identity: true),
                        IngredientID = c.String(nullable: false, maxLength: 255),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.StockID)
                .ForeignKey("dbo.Ingredients", t => t.IngredientID, cascadeDelete: true)
                .Index(t => t.IngredientID);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierID = c.String(nullable: false, maxLength: 255),
                        SupplierName = c.String(nullable: false, maxLength: 255),
                        Phone = c.String(maxLength: 20),
                        Email = c.String(maxLength: 255),
                        Address = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.SupplierID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Carts", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Recipis", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Ingredients", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.Recipis", "IngredientID", "dbo.Ingredients");
            DropForeignKey("dbo.IngredientStocks", "IngredientID", "dbo.Ingredients");
            DropForeignKey("dbo.PromotionDetails", "PromotionID", "dbo.Promotions");
            DropForeignKey("dbo.PromotionDetails", "ProductID", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Payments", "PaymentMethodID", "dbo.PaymentMethods");
            DropForeignKey("dbo.Payments", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.OrderDetails", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.EmployeeSales", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Employees", "PositionID", "dbo.Positions");
            DropForeignKey("dbo.EmployeeSales", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeSalarys", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Orders", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.CustomerReviews", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.CartDetails", "ProductID", "dbo.Products");
            DropForeignKey("dbo.CustomerReviews", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.CartDetails", "CartID", "dbo.Carts");
            DropIndex("dbo.IngredientStocks", new[] { "IngredientID" });
            DropIndex("dbo.Ingredients", new[] { "SupplierID" });
            DropIndex("dbo.Recipis", new[] { "IngredientID" });
            DropIndex("dbo.Recipis", new[] { "ProductID" });
            DropIndex("dbo.PromotionDetails", new[] { "ProductID" });
            DropIndex("dbo.PromotionDetails", new[] { "PromotionID" });
            DropIndex("dbo.Payments", new[] { "PaymentMethodID" });
            DropIndex("dbo.Payments", new[] { "OrderID" });
            DropIndex("dbo.EmployeeSalarys", new[] { "EmployeeID" });
            DropIndex("dbo.Employees", new[] { "PositionID" });
            DropIndex("dbo.EmployeeSales", new[] { "OrderID" });
            DropIndex("dbo.EmployeeSales", new[] { "EmployeeID" });
            DropIndex("dbo.Orders", new[] { "CustomerID" });
            DropIndex("dbo.OrderDetails", new[] { "ProductID" });
            DropIndex("dbo.OrderDetails", new[] { "OrderID" });
            DropIndex("dbo.Products", new[] { "CategoryID" });
            DropIndex("dbo.CustomerReviews", new[] { "ProductID" });
            DropIndex("dbo.CustomerReviews", new[] { "CustomerID" });
            DropIndex("dbo.Carts", new[] { "CustomerID" });
            DropIndex("dbo.CartDetails", new[] { "ProductID" });
            DropIndex("dbo.CartDetails", new[] { "CartID" });
            DropTable("dbo.Suppliers");
            DropTable("dbo.IngredientStocks");
            DropTable("dbo.Ingredients");
            DropTable("dbo.Recipis");
            DropTable("dbo.Promotions");
            DropTable("dbo.PromotionDetails");
            DropTable("dbo.PaymentMethods");
            DropTable("dbo.Payments");
            DropTable("dbo.Positions");
            DropTable("dbo.EmployeeSalarys");
            DropTable("dbo.Employees");
            DropTable("dbo.EmployeeSales");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Categories");
            DropTable("dbo.Products");
            DropTable("dbo.CustomerReviews");
            DropTable("dbo.Customers");
            DropTable("dbo.Carts");
            DropTable("dbo.CartDetails");
        }
    }
}
