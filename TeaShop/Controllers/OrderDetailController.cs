using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;
using TeaShop.ViewModel;

namespace TeaShop.Controllers
{
    public class OrderDetailController : Controller
    {
        private DataContext db = new DataContext();

        public ActionResult Index(string[] selectedItems)
        {
            if (selectedItems == null || selectedItems.Length == 0)
            {
                TempData["err"] = "Vui lòng chọn sản phẩm";
                return RedirectToAction("Index", "Home");
            }

            var cartDetails = db.CartDetails
                                .Include(cd => cd.Product)
                                .Where(cd => selectedItems.Contains(cd.CartDetailID.ToString()))
                                .ToList();

            var viewModel = new List<CartViewModel>
    {
        new CartViewModel
        {
            CartDetails = cartDetails
        }
    };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(string paymentMethod, string[] selectedCartDetails)
        {
            if (Session["CustomerID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customerId = Session["CustomerID"].ToString();
            var customer = db.Customers.FirstOrDefault(c => c.CustomerID == customerId);
            if (customer == null)
            {
                ModelState.AddModelError("", "Khách hàng không tồn tại.");
                return RedirectToAction("Index", "Cart");
            }

            var paymentMethodModel = new PaymentMethodModel
            {
                MethodName = paymentMethod,
            };
            db.PaymentMethods.Add(paymentMethodModel);
            db.SaveChanges();

            var newOrder = new OrderModel
            {
                CustomerID = customerId,
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                OrderStatus = "Chờ xử lý",
                OrderDetails = new List<OrderDetailModel>()
            };

            decimal totalAmount = 0;

            // Convert selectedCartDetails to a list of integers
            var selectedIds = selectedCartDetails.Select(int.Parse).ToList();

            var cartDetails = db.CartDetails
                                .Include(cd => cd.Product)
                                .Where(cd => selectedIds.Contains(cd.CartDetailID))
                                .ToList();

            foreach (var cartDetail in cartDetails)
            {
                var product = cartDetail.Product;
                var discountPercentage = db.PromotionDetails

        .Where(pd => pd.ProductID == product.ProductID)
        .Select(pd => pd.Promotion.DiscountPercentage)
        .FirstOrDefault();

                var price = discountPercentage > 0
                    ? product.Price * (1 - discountPercentage / 100m)
                    : product.Price;
                // Check stock availability
                if (product.Stock < cartDetail.Quantity)
                {
                    // If stock is insufficient, return an error message
                    ViewBag.ErrorMessage = $"Không đủ số lượng sản phẩm {product.ProductName}. Có sẵn: {product.Stock}, yêu cầu: {cartDetail.Quantity}";
                    return View("Payment"); // Return to the payment view with the error message
                }

                // Deduct the quantity from the stock
                product.Stock -= cartDetail.Quantity;

                var orderDetail = new OrderDetailModel
                {
                    OrderID = newOrder.OrderID,
                    ProductID = cartDetail.ProductID,
                    Quantity = cartDetail.Quantity,
                    Price = price, // Assuming Price is a property of Product cartDetail.Product.Price
                    Size = cartDetail.Size
                };

                totalAmount += orderDetail.Quantity * orderDetail.Price;
                newOrder.OrderDetails.Add(orderDetail);

                db.CartDetails.Remove(cartDetail); // Remove the selected items from the cart
            }

            newOrder.TotalAmount = totalAmount;
            db.Orders.Add(newOrder);
            db.SaveChanges();
            TempData["success"] = "Thanh toán thành công!";
            return RedirectToAction("OrderConfirmation", new { id = newOrder.OrderID });
        }
        public ActionResult OrderConfirmation(int? id)
        {
            // Kiểm tra nếu như người dùng chưa đăng nhập
            if (Session["CustomerID"] == null)
            {
                // Thiết lập thông báo trong ViewBag
                ViewBag.AlertMessage = "Bạn cần đăng nhập để xem đơn hàng.";
                return RedirectToAction("Login", "Account");
            }

            // Lấy CustomerID từ session
            string customerId = Session["CustomerID"].ToString();

            // Lấy tất cả các đơn hàng của khách hàng
            var allOrders = db.Orders
                              .Include(o => o.OrderDetails.Select(od => od.Product))
                              .Include(o => o.Customer)
                              .Where(o => o.CustomerID == customerId)
                              .OrderByDescending(o => o.OrderDate)
                              .ToList();

            // Đánh dấu đơn hàng mới nhất nếu có id hợp lệ
            int? latestOrderId = null;
            if (id.HasValue && id.Value > 0)
            {
                latestOrderId = id.Value;
            }

            // Tạo danh sách OrderViewModel để trả về View
            var allOrdersViewModel = allOrders.Select(order => new OrderViewModel
            {
                OrderID = order.OrderID,
                CustomerName = order.Customer.CustomerName,
                Phone = order.Customer.Phone,
                Email = order.Customer.Email,
                Address = order.Customer.Address,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                IsLatestOrder = order.OrderID == latestOrderId,
                OrderItems = order.OrderDetails.Select(od => new OrderItemViewModel
                {
                    ProductName = od.Product.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.Price,
                    Size = od.Size
                }).ToList()
            }).ToList();

            // Trả về View với danh sách tất cả đơn hàng của khách hàng
            return View(allOrdersViewModel);
        }
    }
}


//        public ActionResult OrderConfirmation(int id)
//        {

//            var order = db.Orders
//                          .Include(o => o.OrderDetails.Select(od => od.Product))
//                          .Include(o => o.Customer)
//                          .FirstOrDefault(o => o.OrderID == id);
//            id = 0;
//            if (id<=0)
//            {

//                return View(order);
//            }
//            else { 


//            if (order == null)
//            {
//                return RedirectToAction("Index", "Home");
//            }

//            var orderViewModel = new OrderViewModel
//            {
//                OrderID = order.OrderID,
//                CustomerName = order.Customer.CustomerName,
//                Phone = order.Customer.Phone,
//                Email = order.Customer.Email,
//                Address = order.Customer.Address,
//                OrderDate = order.OrderDate,
//                TotalAmount = order.TotalAmount,
//                OrderStatus = order.OrderStatus,
//                OrderItems = order.OrderDetails.Select(od => new OrderItemViewModel
//                {
//                    ProductID = od.ProductID,
//                    Quantity = od.Quantity,
//                    UnitPrice = od.Price,
//                    Size = od.Size
//                }).ToList()
//            };

//            return View(orderViewModel);
//        }
//    }
//    }
//}
