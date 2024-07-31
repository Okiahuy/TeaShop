using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.ViewModel;
using System.Data.Entity;
using TeaShop.Respository;
using Microsoft.Ajax.Utilities;

namespace TeaShop.Areas.Admin.Controllers
{
    public class OrderAdminController : Controller
    {
        private readonly DataContext db = new DataContext();

        // GET: Admin/OrderAdmin
        public ActionResult Index(string searchString)
        {
            var orders = db.Orders.Include(o => o.Customer).ToList();
            var products = db.Products.ToList();
            ViewBag.ProductList = new SelectList(products, "ProductID", "ProductName");

            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => o.Customer.CustomerID.Contains(searchString)).ToList();
            }

            var orderViewModels = orders.Select(order => new OrderViewModel
            {
                OrderID = order.OrderID,
                CustomerName = order.Customer.CustomerName,
                Phone = order.Customer.Phone,
                Email = order.Customer.Email,
                Address = order.Customer.Address,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                OrderItems = order.OrderDetails.Select(od => new OrderItemViewModel
                {
                    ProductID = od.ProductID,
                    Quantity = od.Quantity,
                    UnitPrice = od.Price
                }).ToList()
            }).ToList();

            return View(orderViewModels);
        }

        // GET: Admin/OrderAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/OrderAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if the customer already exists
                var existingCustomer = db.Customers.FirstOrDefault(c => c.Phone == orderViewModel.Phone);

                // Create a new customer if not exists
                if (existingCustomer == null)
                {
                    existingCustomer = new CustomerModel
                    {
                        CustomerID = GenerateCustomerID(), // Generate a new ID
                        CustomerName = orderViewModel.CustomerName,
                        Email = orderViewModel.Email,
                        Phone = orderViewModel.Phone,
                        Address = orderViewModel.Address,
                        Username = "Khach", // Default username
                        PasswordHash = "123"  // No password for guest
                    };


                }
                db.Customers.Add(existingCustomer);
                 db.SaveChanges();
                    
                   
                // Create a new order
                var newOrder = new OrderModel
                {
                    CustomerID = existingCustomer.CustomerID,
                    OrderDate = DateTime.Now,
                    TotalAmount = orderViewModel.TotalAmount,
                    OrderStatus = orderViewModel.OrderStatus,
                };
                db.Orders.Add(newOrder);
                db.SaveChanges();

                // Thêm chi tiết đơn hàng
                foreach (var item in orderViewModel.OrderItems)
                {
                    var orderDetail = new OrderDetailModel
                    {
                        OrderID = newOrder.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Price = item.UnitPrice,
                        Size = item.Size, // Đảm bảo thuộc tính Size có giá trị đúng
                    };

                    db.OrderDetails.Add(orderDetail);
                }

                db.SaveChanges();

                return RedirectToAction("OrderConfirmation", new { id = newOrder.OrderID });
            }

            // If the model state is not valid, reload the form with existing data
            return View(orderViewModel);
        }

        private string GenerateCustomerID()
        {
            var latestCustomer = db.Customers.OrderByDescending(c => c.CustomerID).FirstOrDefault();
            if (latestCustomer == null)
            {
                return "KHTQ001";
            }

            string latestId = latestCustomer.CustomerID;
            int latestNumber;

            if (latestId.StartsWith("KHTQ") && latestId.Length == 7 && int.TryParse(latestId.Substring(4), out latestNumber))
            {
                int newNumber = latestNumber + 1;
                return "KHTQ" + newNumber.ToString("D3");
            }
            else
            {
                return "KHTQ001";
            }
        }
    }
}
