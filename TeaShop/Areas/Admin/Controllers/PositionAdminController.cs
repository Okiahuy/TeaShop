using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;

namespace TeaShop.Areas.Admin.Controllers
{
    public class PositionAdminController : Controller
    {
        private readonly DataContext db = new DataContext(); // Assuming ApplicationDbContext is your DbContext

        // GET: Admin/PositionAdmin
        public ActionResult Index(string searchString)
        {
            var positions = from p in db.Positions
                            select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                positions = positions.Where(p => p.PositionName.Contains(searchString));
            }

            return View(positions.ToList());
        }

        // POST: Admin/PositionAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PositionModel position)
        {
            if (ModelState.IsValid)
            {
                position.PositionID=GenerateNewPositionID();
                db.Positions.Add(position);
                db.SaveChanges();
                return RedirectToAction("Index", "PositionAdmin");
            }
            return View(position);
        }

        // POST: Admin/PositionAdmin/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PositionModel position)
        {
            if (ModelState.IsValid)
            {
                var existingPosition = db.Positions.Find(position.PositionID);
                if (existingPosition == null)
                {
                    return HttpNotFound();
                }

                existingPosition.PositionName = position.PositionName;

                db.Entry(existingPosition).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "PositionAdmin");
            }
            return View(position);
        }

        // POST: Admin/PositionAdmin/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string PositionID)
        {
            var position = db.Positions.Find(PositionID);
            if (position == null)
            {
                return HttpNotFound();
            }

            db.Positions.Remove(position);
            db.SaveChanges();
            return RedirectToAction("Index", "PositionAdmin");
        }

        private string GenerateNewPositionID()
        {
            var prefix = "CV";
            var lastPosition = db.Positions.OrderByDescending(p => p.PositionID).FirstOrDefault();
            int nextNumber = 1;

            if (lastPosition != null)
            {
                var lastNumberPart = lastPosition.PositionID.Substring(prefix.Length);
                if (int.TryParse(lastNumberPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:001}";
        }
    }
}