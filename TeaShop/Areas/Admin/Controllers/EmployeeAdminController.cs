using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;
using System.Data.Entity;

namespace TeaShop.Areas.Admin.Controllers
{
    public class EmployeeAdminController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Admin/EmployeeAdmin
        public ActionResult Index(string searchString)
        {
            // Fetch all employees from the database
            var employees = db.Employees.ToList();

            // If a search string is provided, filter the employees list
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.EmployeeName.Contains(searchString)).ToList();
            }


            var positions = db.Positions.ToList();
            ViewBag.PositionList = new SelectList(positions, "PositionID", "PositionName");

            // Return the view with the filtered or unfiltered list of employees
            return View(employees);
        }

     
        // POST: Admin/EmployeeAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeModel employee)
        {
            // Tính tuổi của nhân viên
            int age = (int)((DateTime.Now - employee.HireDate).TotalDays / 365.25);

            if (ModelState.IsValid)
            {
                /*if (age < 18)
                {
                    // Thêm thông báo lỗi vào ModelState
                    TempData["err"] = "Nhân viên phải từ đủ 18 tuổi trở lên";
                }*/

                if (ModelState.IsValid)
                {
                    
                    db.Employees.Add(employee);
                    db.SaveChanges();
                    TempData["success"] = "Thêm nhân vien thành công";
                    return RedirectToAction("Index", "EmployeeAdmin");
                }
            }
            var positions = db.Positions.ToList();
            ViewBag.PositionList = new SelectList(positions, "PositionID", "PositionName");
            return View(employee);
        }


        // POST: Admin/EmployeeAdmin/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeModel employee)
        {
            if (ModelState.IsValid)
            {
                var getemployee = db.Employees.Find(employee.EmployeeID);
                getemployee.EmployeeID = employee.EmployeeID;
                getemployee.Position = employee.Position;
                getemployee.Phone = employee.Phone;
                getemployee.Email = employee.Email;
                getemployee.Address = employee.Address;
                getemployee.HireDate = employee.HireDate;
             
                db.SaveChanges();
                return RedirectToAction("Index", "EmployeeAdmin");
            }
            return View(employee);
        }
        //public ActionResult Delete(int EmployeeID)
        //{
        //    if (EmployeeID == null)
        //    {
        //        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        //    }

        //    var employ = db.Employees.Find(EmployeeID);
        //    if (employ == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(employ);
        //}
        // POST: Admin/EmployeeAdmin/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int EmployeeID)
        {
            var employee = db.Employees.Find(EmployeeID);
            if(employee == null)
            {
                return HttpNotFound();
            }
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}