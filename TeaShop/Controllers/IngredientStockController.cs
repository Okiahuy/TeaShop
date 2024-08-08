using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;
using System.Data.Entity;

namespace TeaShop.Controllers
{
    public class IngredientStockController : Controller
    {
        private readonly DataContext db = new DataContext();

        // GET: IngredientStock
        public ActionResult Index(string searchString)
        {
            var stocks = db.IngredientStocks.Include("Ingredient").AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                stocks = stocks.Where(s => s.Ingredient.IngredientName.Contains(searchString));
            }

            return View(stocks.ToList());
        }

        // GET: IngredientStock/Create
        public ActionResult Create()
        {
            ViewBag.IngredientID = new SelectList(db.Ingredients, "IngredientID", "IngredientName");
            return View();
        }

        // POST: IngredientStock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockID,IngredientID,Quantity,StockDate")] IngredientStockModel ingredientStock)
        {
            if (ModelState.IsValid)
            {
                db.IngredientStocks.Add(ingredientStock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IngredientID = new SelectList(db.Ingredients, "IngredientID", "IngredientName", ingredientStock.IngredientID);
            return View(ingredientStock);
        }

        // GET: IngredientStock/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientStockModel ingredientStock = db.IngredientStocks.Find(id);
            if (ingredientStock == null)
            {
                return HttpNotFound();
            }
            ViewBag.IngredientID = new SelectList(db.Ingredients, "IngredientID", "IngredientName", ingredientStock.IngredientID);
            return View(ingredientStock);
        }

        // POST: IngredientStock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockID,IngredientID,Quantity,StockDate")] IngredientStockModel ingredientStock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ingredientStock).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IngredientID = new SelectList(db.Ingredients, "IngredientID", "IngredientName", ingredientStock.IngredientID);
            return View(ingredientStock);
        }

        // GET: IngredientStock/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientStockModel ingredientStock = db.IngredientStocks.Find(id);
            if (ingredientStock == null)
            {
                return HttpNotFound();
            }
            return View(ingredientStock);
        }

        // POST: IngredientStock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IngredientStockModel ingredientStock = db.IngredientStocks.Find(id);
            db.IngredientStocks.Remove(ingredientStock);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
