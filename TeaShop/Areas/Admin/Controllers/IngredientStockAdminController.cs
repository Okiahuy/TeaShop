using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;

namespace TeaShop.Areas.Admin.Controllers
{
    public class IngredientStockAdminController : Controller
    {
        private readonly DataContext db = new DataContext();

        // GET: Admin/IngredientStockAdmin
        public ActionResult Index(string searchString)
        {
            // Load all ingredient stocks including related ingredients
            var stocks = db.IngredientStocks.Include("Ingredient").AsQueryable();

            // Filter by search string if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                stocks = stocks.Where(s => s.Ingredient.IngredientName.Contains(searchString));
            }

            // Load all ingredients for the dropdown list
            var ingredients = db.Ingredients.Select(i => new
            {
                IngredientID = i.IngredientID,
                IngredientName = i.IngredientName
            }).ToList();

            // Create a SelectList to pass to the view
            ViewBag.IngredientList = new SelectList(ingredients, "IngredientID", "IngredientName");

            return View(stocks.ToList());
        }

        // POST: Admin/IngredientStockAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IngredientID,Quantity,StockDate")] IngredientStockModel ingredientStock)
        {
            if (ModelState.IsValid)
            {
                db.IngredientStocks.Add(ingredientStock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Reload ingredients for dropdown
            var ingredients = db.Ingredients.Select(i => new
            {
                IngredientID = i.IngredientID,
                IngredientName = i.IngredientName
            }).ToList();
            ViewBag.IngredientList = new SelectList(ingredients, "IngredientID", "IngredientName");

            return View("Index", db.IngredientStocks.ToList());
        }

        // GET: Admin/IngredientStockAdmin/Edit/5
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

            // Load all ingredients for the dropdown list
            var ingredients = db.Ingredients.Select(i => new
            {
                IngredientID = i.IngredientID,
                IngredientName = i.IngredientName
            }).ToList();
            ViewBag.IngredientList = new SelectList(ingredients, "IngredientID", "IngredientName");

            return View(ingredientStock);
        }

        // POST: Admin/IngredientStockAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockID,IngredientID,Quantity,StockDate")] IngredientStockModel ingredientStock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ingredientStock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Reload ingredients for dropdown
            var ingredients = db.Ingredients.Select(i => new
            {
                IngredientID = i.IngredientID,
                IngredientName = i.IngredientName
            }).ToList();
            ViewBag.IngredientList = new SelectList(ingredients, "IngredientID", "IngredientName");

            return View(ingredientStock);
        }

        // POST: Admin/IngredientStockAdmin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int StockID)
        {
            IngredientStockModel ingredientStock = db.IngredientStocks.Find(StockID);
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
