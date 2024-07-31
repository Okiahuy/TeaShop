

using System.Linq;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;

namespace TeaShop.Areas.Admin.Controllers
{
    public class CustomerAdminController : Controller
    {
        private readonly DataContext db = new DataContext();

        // GET: Admin/CustomerAdmin
        public ActionResult Index(string CustomerName)
        {
            var customers = string.IsNullOrEmpty(CustomerName)
                ? db.Customers.ToList()
                : db.Customers.Where(c => c.CustomerName.Contains(CustomerName)).ToList();

            return View(customers);
        }


        public ActionResult Create()
        {
            return View();
        }
        // POST: Admin/CustomerAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerModel customerModel)
        {



            if (!ModelState.IsValid)
            {
                ViewBag.ErrCreateCustomer = "Dữ liệu không hợp lệ";
                var customers = db.Customers.ToList();
                return View("Index", customers);
            }

            if (db.Customers.Any(c => c.CustomerID == customerModel.CustomerID))
            {
                ViewBag.ErrCreateCustomer = "Customer ID đã tồn tại";
                var customers = db.Customers.ToList();
                return View("Index", customers);
            }
            customerModel.CustomerID = GenerateCustomerID();
            db.Customers.Add(customerModel);
            db.SaveChanges();


            return RedirectToAction("Index");
        }


        private string GenerateCustomerID()
        {
            // Logic to generate a unique ProductID
            var lastProduct = db.Customers.OrderByDescending(p => p.CustomerID).FirstOrDefault();
            if (lastProduct != null)
            {
                int lastID = int.Parse(lastProduct.CustomerID.Substring(2)); // Adjust the substring as needed
                return "KH" + (lastID + 1).ToString("D3");
            }
            else
            {
                return "KH001";
            }
        }
    }
}

