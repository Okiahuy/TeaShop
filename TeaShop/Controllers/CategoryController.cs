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
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        public CategoryController(DataContext context)
        {
            _context = context;
        }
        public ActionResult siderPartial()
        {
            try
            {
                var categories = _context.Categories.ToList();
                return PartialView("_siderPartial", categories);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi hoặc xử lý lỗi theo nhu cầu của bạn
                throw new HttpException(500, "Internal Server Error", ex);
            }
        }

        public ActionResult GetCategory(string CateID = "")
        {
            var ProductList = _context.Products.Include(c => c.Category).AsQueryable();
            if (!string.IsNullOrEmpty(CateID))
            {
                ProductList = ProductList.Where(c => c.CategoryID == CateID);
            }

            var CategoryList = _context.Categories.ToList();

            var viewModel = new MainViewModel
            {

                CategoryList = CategoryList
            };

            return View(viewModel);
        }
    }
}