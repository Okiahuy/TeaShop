using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;
using System.Data.Entity;
using TeaShop.ViewModel;

namespace TeaShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context= new DataContext();

        public ActionResult Index()
        {

            try
            {
                var promotedProducts = _context.PromotionDetails
                                               .Include(pd => pd.Product)
                                               .Include(pd => pd.Promotion)
                                               .Where(pd => pd.Promotion.EndDate >= DateTime.Now)
                                               .Select(pd => new PromotedProductViewModel
                                               {
                                                   ProductID = pd.Product.ProductID,
                                                   Name = pd.Product.ProductName,
                                                   Description = pd.Product.Description,
                                                   Price = pd.Product.Price,
                                                   DiscountPercentage = pd.Promotion.DiscountPercentage,
                                                   EndDate = pd.Promotion.EndDate,
                                                   ImageURL = pd.Product.ImageURL
                                               })
                                               .ToList();

               var productmodel = _context.Products.Include(p => p.Category).Take(12)

                                                 .ToList();
                var model = new HomeViewModel
                {
                    PromotedProducts = promotedProducts,
                    ProductModels = productmodel
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                ViewBag.ErrorMessage = "An error occurred while fetching promotions.";
                return View("Error");
            }


        }




        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}