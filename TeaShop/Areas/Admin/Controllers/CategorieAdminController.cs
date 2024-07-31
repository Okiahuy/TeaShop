
using System.Linq;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;
using TeaShop.ViewModel;

namespace TeaShop.Areas.Admin.Controllers
{
    public class CategorieAdminController : Controller
    {
        private readonly DataContext db = new DataContext();

        // GET: Admin/CategorieAdmin
        public ActionResult Index(string CategoryName)
        {
            var categories = db.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(CategoryName))
            {
                categories = categories.Where(c => c.CategoryName.Contains(CategoryName));
            }

            var viewModel = categories.Select(c => new CateViewModel
            {
                CategoryID = c.CategoryID,
                CategoryName = c.CategoryName,
                Description = c.Description
            }).ToList();

            return View(viewModel);
        }

        // GET: Admin/CategorieAdmin/Create
        public ActionResult Create()
        {
            ViewBag.NewCategoryID = GenerateCategoryID();
            return View();
        }

        // POST: Admin/CategorieAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategorieModel model)
        {
            if (ModelState.IsValid)
            {
                // Sinh mã mới nếu cần thiết, đảm bảo rằng mã không bị thay đổi nếu đã có
                if (string.IsNullOrEmpty(model.CategoryID))
                {
                    model.CategoryID = GenerateCategoryID();
                    ViewBag.NewCategoryID = model.CategoryID ?? GenerateCategoryID();
                }

                db.Categories.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Trả lại mã hiện tại trong trường hợp có lỗi

            return View(model);
        }

        private string GenerateCategoryID()
        {
            // Logic to generate a unique ProductID
            var lastProduct = db.Categories.OrderByDescending(p => p.CategoryID).FirstOrDefault();
            if (lastProduct != null)
            {
                int lastID = int.Parse(lastProduct.CategoryID.Substring(3)); // Adjust the substring as needed
                return "CAT" + (lastID + 1).ToString("D3");
            }
            else
            {
                return "CAT001";
            }
        }
        // GET: Admin/CategorieAdmin/Edit/{id}
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // POST: Admin/CategorieAdmin/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategorieModel model)
        {
            if (ModelState.IsValid)
            {
                var category = db.Categories.Find(model.CategoryID);
                if (category == null)
                {
                    return HttpNotFound();
                }

                category.CategoryName = model.CategoryName;
                category.Description = model.Description;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Admin/CategorieAdmin/Delete/{id}
        public ActionResult Delete(string CategoryID)
        {
            if (CategoryID == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var category = db.Categories.Find(CategoryID);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // POST: Admin/CategorieAdmin/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string CategoryID)
        {
            var category = db.Categories.Find(CategoryID);
            if (category == null)
            {
                return HttpNotFound();
            }

            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
