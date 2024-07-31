using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;

namespace TeaShop.Areas.Admin.Controllers
{
    public class SupplierAdminController : Controller
    {
        private readonly DataContext db = new DataContext();

        // GET: SupplierAdmin
        public ActionResult Index(string searchString)
        {
            var suppliers = db.Suppliers.ToList();


            if (!String.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(s => s.SupplierName.Contains(searchString)).ToList();
            }

            return View(suppliers);
        }

        // POST: SupplierAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SupplierModel supplier)
        {
            if (ModelState.IsValid)
            {
                supplier.SupplierID = GenerateSupplierID(); // Generate unique ID
                db.Suppliers.Add(supplier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(supplier);
        }

        // POST: SupplierAdmin/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SupplierModel supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // POST: SupplierAdmin/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string SupplierID)
        {
            var supplier = db.Suppliers.Find(SupplierID);
            if(supplier == null)
            {
                HttpNotFound();
            }
            db.Suppliers.Remove(supplier);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private string GenerateSupplierID()
        {
            var latestSupplier = db.Suppliers.OrderByDescending(s => s.SupplierID).FirstOrDefault();
            if (latestSupplier == null)
            {
                return "NCC001";
            }

            string latestId = latestSupplier.SupplierID;
            int latestNumber = int.Parse(latestId.Substring(3));
            int newNumber = latestNumber + 1;

            return "NCC" + newNumber.ToString("D3");
        }

    }
}