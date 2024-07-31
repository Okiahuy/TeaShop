using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;
using System.Data.Entity;

namespace TeaShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context= new DataContext();
        
        public ActionResult Index()
        {
            //var products = _context.Products.Include(p => p.Category)
            //                                  .Where(p => p.CategoryID == "CAT004")
            //                                  .Take(6)
            //                                 .ToList();
            //var model = new HomeViewModel
            //{
            //    ProductModels = products
            //};
            //return View(model);
            return View();
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