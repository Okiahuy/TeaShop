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
    public class PromotionAdminController : Controller
    {
        // GET: Admin/PromotionAdmin

        private readonly DataContext _context = new DataContext();
        public ActionResult Index(string searchString)
        {
            var promotions = _context.Promotions.ToList();
                             

            if (!string.IsNullOrEmpty(searchString))
            {
                promotions = promotions.Where(p => p.PromotionName.Contains(searchString)).ToList();
            }

            return View(promotions);
        }

        // GET: Admin/PromotionAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/PromotionAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PromotionModel promotion)
        {
            if (ModelState.IsValid)
            {
                promotion.PromotionID = GeneratePromotionID();
                _context.Promotions.Add(promotion);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(promotion);
        }

        // GET: Admin/PromotionAdmin/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var promotion = _context.Promotions.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }

            return View(promotion);
        }

        // POST: Admin/PromotionAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PromotionModel promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(promotion).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(promotion);
        }

        // POST: Admin/PromotionAdmin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string PromotionID)
        {
            var promotion = _context.Promotions.Find(PromotionID);
            if (promotion == null)
            {
                return HttpNotFound();
            }

            _context.Promotions.Remove(promotion);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Helper method to generate PromotionID
        private string GeneratePromotionID()
        {
            var latestPromotion = _context.Promotions.OrderByDescending(p => p.PromotionID).FirstOrDefault();
            if (latestPromotion == null)
            {
                return "KM001";
            }

            string latestId = latestPromotion.PromotionID;
            int latestNumber = int.Parse(latestId.Substring(2));
            int newNumber = latestNumber + 1;

            return "KM" + newNumber.ToString("D3");
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