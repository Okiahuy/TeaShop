using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeaShop.Respository;
using TeaShop.ViewModel;

namespace TeaShop.Areas.Admin.Controllers
{
    public class StatisticalAdminController : Controller
    {
        private readonly DataContext _context = new DataContext();
        public ActionResult Index()
        {
            var totalCost = _context.IngredientStocks
                .Join(_context.Ingredients,
                      stock => stock.IngredientID,
                      ingredient => ingredient.IngredientID,
                      (stock, ingredient) => new { stock.Quantity, ingredient.Price })
                .Sum(item => item.Quantity * item.Price);

            var totalRevenue = _context.Orders
                .Where(o => o.OrderDate.Month == DateTime.Now.Month)
                .Sum(o => o.TotalAmount);

            var viewModel = new StatisticsViewModel
            {
                TotalCost = totalCost,
                TotalRevenue = totalRevenue,
                NetRevenue = totalRevenue - totalCost,
                TotalCustomers = _context.Orders
                    .Where(o => o.OrderDate.Month == DateTime.Now.Month)
                    .Select(o => o.CustomerID)
                    .Distinct()
                    .Count(),
                InStoreCustomers = _context.Orders
                    .Where(o => o.OrderDate.Month == DateTime.Now.Month && o.CustomerID.StartsWith("KHTQ"))
                    .Select(o => o.CustomerID)
                    .Distinct()
                    .Count(),
                OnlineCustomers = _context.Orders
                    .Where(o => o.OrderDate.Month == DateTime.Now.Month && o.CustomerID.StartsWith("KH"))
                    .Select(o => o.CustomerID)
                    .Distinct()
                    .Count()
            };

            return View(viewModel);
        }
    }
}