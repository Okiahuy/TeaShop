using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;

namespace TeaShop.Areas.Admin.Controllers
{
    public class PromotionDetailAdminController : Controller
    {
        // GET: Admin/PromotionDetailAdmin
        private readonly DataContext _context = new DataContext();
        public ActionResult Index(string searchString)
        {
            var promotionDetails = _context.PromotionDetails.Include("Promotion").Include("Product").ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                promotionDetails = promotionDetails.Where(pd => pd.Promotion.PromotionName.Contains(searchString) || pd.Product.ProductName.Contains(searchString)).ToList();
            }
            ViewBag.PromotionID = new SelectList(_context.Promotions, "PromotionID", "PromotionName");
            ViewBag.ProductID = new SelectList(_context.Products, "ProductID", "ProductName");
            return View(promotionDetails.ToList());
        }

        // GET: Admin/PromotionDetailAdmin/Create
        public ActionResult Create()
        {
            ViewBag.PromotionID = new SelectList(_context.Promotions, "PromotionID", "PromotionName");
            ViewBag.ProductID = new SelectList(_context.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: Admin/PromotionDetailAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PromotionDetailModel promotionDetail)
        {
            if (ModelState.IsValid)
            {
                _context.PromotionDetails.Add(promotionDetail);
                _context.SaveChanges();
                TempData["success"] = "Thêm khuyến mãi thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.PromotionID = new SelectList(_context.Promotions, "PromotionID", "PromotionName", promotionDetail.PromotionID);
            ViewBag.ProductID = new SelectList(_context.Products, "ProductID", "ProductName", promotionDetail.ProductID);
            return View(promotionDetail);
        }

        // GET: Admin/PromotionDetailAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            var promotionDetail = _context.PromotionDetails.Find(id);
            if (promotionDetail == null)
            {
                return HttpNotFound();
            }
            //TempData["success"] = "Sửa khuyến mãi thành công!";
            ViewBag.PromotionID = new SelectList(_context.Promotions, "PromotionID", "PromotionName", promotionDetail.PromotionID);
            ViewBag.ProductID = new SelectList(_context.Products, "ProductID", "ProductName", promotionDetail.ProductID);
            return View(promotionDetail);
        }

        // POST: Admin/PromotionDetailAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PromotionDetailModel promotionDetail)
        {
            if (ModelState.IsValid)
            {
                TempData["success"] = "Sửa khuyến mãi thành công!";
                _context.Entry(promotionDetail).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PromotionID = new SelectList(_context.Promotions, "PromotionID", "PromotionName", promotionDetail.PromotionID);
            ViewBag.ProductID = new SelectList(_context.Products, "ProductID", "ProductName", promotionDetail.ProductID);
            return View(promotionDetail);
        }

        // POST: Admin/PromotionDetailAdmin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int PromotionDetailID)
        {
            var promotionDetail = _context.PromotionDetails.Find(PromotionDetailID);
            if (promotionDetail == null)
            {
                return HttpNotFound();
            }
            TempData["success"] = "Xóa khuyến mãi thành công!";
            _context.PromotionDetails.Remove(promotionDetail);
            _context.SaveChanges();
            return RedirectToAction("Index");
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