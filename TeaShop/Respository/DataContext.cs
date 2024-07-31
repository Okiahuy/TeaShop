using TeaShop.Models;
using System.Data.Entity;

namespace TeaShop.Respository
{
    public class DataContext:DbContext
    {
        public DataContext() : base("name=TeaShopDbContext")
        {
        }

        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategorieModel> Categories { get; set; }
        public DbSet<PromotionModel> Promotions { get; set; }
        public DbSet<PromotionDetailModel> PromotionDetails { get; set; }
        public DbSet<CustomerReviewModel> CustomerReviews { get; set; }
        public DbSet<SupplierModel> Suppliers { get; set; }
        public DbSet<IngredientModel> Ingredients { get; set; }
        public DbSet<RecipiModel> Recipes { get; set; }
        public DbSet<IngredientStockModel> IngredientStocks { get; set; }
        public DbSet<PositionModel> Positions { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<EmployeeSaleModel> EmployeeSales { get; set; }
        public DbSet<EmployeeSalaryModel> EmployeeSalaries { get; set; }
        public DbSet<CartModel> Carts { get; set; }
        public DbSet<CartDetailModel> CartDetails { get; set; }
        public DbSet<PaymentMethodModel> PaymentMethods { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình decimal(18, 2) cho các trường decimal
            modelBuilder.Entity<OrderModel>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetailModel>()
                .Property(od => od.Price)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductModel>()
                .Property(p => p.Price)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<IngredientModel>()
                .Property(i => i.Price)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<RecipiModel>()
                .Property(r => r.Quantity)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<IngredientStockModel>()
                .Property(s => s.Quantity)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<CartDetailModel>()
                .Property(cd => cd.Price)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<PaymentModel>()
                .Property(p => p.Amount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<EmployeeSalaryModel>()
                .Property(es => es.SalaryAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // Cấu hình mối quan hệ
            modelBuilder.Entity<OrderModel>()
                .HasMany(o => o.OrderDetails)
                .WithRequired(od => od.Order)
                .HasForeignKey(od => od.OrderID);

            modelBuilder.Entity<ProductModel>()
                .HasMany(p => p.OrderDetails)
                .WithRequired(od => od.Product)
                .HasForeignKey(od => od.ProductID);
        }
    }
}
