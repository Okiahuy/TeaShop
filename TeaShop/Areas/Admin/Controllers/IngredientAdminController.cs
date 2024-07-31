using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;

namespace TeaShop.Areas.Admin.Controllers
{
    public class IngredientAdminController : Controller
    {
        // GET: Admin/IngredientAdmin

        private readonly DataContext _context=new DataContext();
        public ActionResult Index(string searchString)
        {
            var ingredients = _context.Ingredients.ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                ingredients = ingredients.Where(i => i.IngredientName.Contains(searchString)).ToList();
            }
            ViewBag.SupplierID = new SelectList(_context.Suppliers, "SupplierID", "SupplierName");
            return View(ingredients);
        }

        // GET: Admin/IngredientAdmin/Create
        public ActionResult Create()
        {
            ViewBag.SupplierID = new SelectList(_context.Suppliers, "SupplierID", "SupplierName");
            return View();
        }

        // POST: Admin/IngredientAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IngredientModel ingredient)
        {
            if (ModelState.IsValid)
            {
                ingredient.IngredientID = GenerateIngredientID();
                _context.Ingredients.Add(ingredient);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SupplierID = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", ingredient.SupplierID);
            return View(ingredient);
        }

        // GET: Admin/IngredientAdmin/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ingredient = _context.Ingredients.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }

            ViewBag.SupplierID = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", ingredient.SupplierID);
            return View(ingredient);
        }

        // POST: Admin/IngredientAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IngredientModel ingredient)
        {
         
            if (ModelState.IsValid)
            {
                _context.Entry(ingredient).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SupplierID = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", ingredient.SupplierID);
            return View(ingredient);
        }

        // POST: Admin/IngredientAdmin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string IngredientID)
        {
            var ingredient = _context.Ingredients.Find(IngredientID);
            if (ingredient == null)
            {
                return HttpNotFound();
            }

            _context.Ingredients.Remove(ingredient);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Helper method to generate IngredientID
        private string GenerateIngredientID()
        {
            var latestIngredient = _context.Ingredients.OrderByDescending(i => i.IngredientID).FirstOrDefault();
            if (latestIngredient == null)
            {
                return "NL001";
            }

            string latestId = latestIngredient.IngredientID;
            int latestNumber = int.Parse(latestId.Substring(2));
            int newNumber = latestNumber + 1;

            return "NL" + newNumber.ToString("D3");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}