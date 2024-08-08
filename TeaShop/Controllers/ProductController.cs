using TeaShop.Models;
using TeaShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using TeaShop.Respository;
using System.Runtime.InteropServices;

namespace TeaShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        public ActionResult Promotion()
        {
            try
            {
                var promotedProducts = _context.PromotionDetails
                                               .Include(pd => pd.Product)
                                               .Include(pd => pd.Promotion)
                                               .Where(pd => pd.Promotion.EndDate >= DateTime.Now)
                                               .Select(pd => new PromotedProductViewModel
                                               {
                                                   ProductID = pd.Product.ProductID,
                                                   Name = pd.Product.ProductName,
                                                   Description = pd.Product.Description,
                                                   Price = pd.Product.Price,
                                                   DiscountPercentage = pd.Promotion.DiscountPercentage,
                                                   EndDate = pd.Promotion.EndDate,
                                                   ImageURL = pd.Product.ImageURL
                                               })
                                               .ToList();

                var model = new HomeViewModel
                {
                    PromotedProducts = promotedProducts
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                ViewBag.ErrorMessage = "An error occurred while fetching promotions.";
                return View("Error");
            }
        }

       

        [HttpPost]
        public ActionResult OrderCafe(OrderDetailModel orderItem)
        {
            try
            {
                // Kiểm tra số lượng phải lớn hơn 0
                if (orderItem.Quantity <= 0)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0." });
                }

                // Tìm sản phẩm dựa trên ProductID
                var product = _context.Products.SingleOrDefault(p => p.ProductID == orderItem.ProductID);
                if (product == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
                }

                // Xác định hệ số giá dựa trên kích cỡ
                decimal sizePriceModifier = 1m; // Hệ số mặc định
                switch (orderItem.Size)
                {
                    case "Hot":
                        sizePriceModifier = 1.0m;
                        break;
                    case "Ice regular size":
                        sizePriceModifier = 1.2m;
                        break;
                    case "Big size ice":
                        sizePriceModifier = 1.5m;
                        break;
                    default:
                        return Json(new { success = false, message = "Kích cỡ không hợp lệ." });
                }

                // Tính tổng giá
                decimal totalPrice = product.Price * sizePriceModifier * orderItem.Quantity;

                // Lấy CustomerID từ session
                var customerId = (string)Session["CustomerID"];
                if (string.IsNullOrEmpty(customerId))
                {
                    return Json(new { success = false, message = "Khách hàng chưa đăng nhập." });
                }

                // Kiểm tra xem Order đã tồn tại chưa
                var order = _context.Orders.SingleOrDefault(o => o.CustomerID == customerId && o.OrderStatus == "Chưa thanh toán");
                if (order == null)
                {
                    // Nếu chưa tồn tại, tạo mới Order
                    order = new OrderModel
                    {
                        CustomerID = customerId,
                        OrderDate = DateTime.Now,
                        TotalAmount = 0, // Sẽ được cập nhật sau
                        OrderStatus = "Chưa thanh toán"
                    };
                    _context.Orders.Add(order);
                    _context.SaveChanges();
                }

                // Tìm OrderDetail nếu đã tồn tại
                var orderDetail = _context.OrderDetails.SingleOrDefault(od => od.OrderID == order.OrderID && od.ProductID == orderItem.ProductID && od.Size == orderItem.Size);

                if (orderDetail != null)
                {
                    // Nếu tồn tại, cập nhật số lượng và giá
                    orderDetail.Quantity += orderItem.Quantity;
                    orderDetail.Price += product.Price * sizePriceModifier * orderItem.Quantity;
                }
                else
                {
                    // Nếu không tồn tại, tạo mới OrderDetail
                    orderDetail = new OrderDetailModel
                    {
                        OrderID = order.OrderID,
                        ProductID = orderItem.ProductID,
                        Size = orderItem.Size,
                        Quantity = orderItem.Quantity,
                        Price = product.Price * sizePriceModifier * orderItem.Quantity,
                    };
                    _context.OrderDetails.Add(orderDetail);
                }

                // Cập nhật tổng tiền của Order
                order.TotalAmount += product.Price * sizePriceModifier * orderItem.Quantity;

                _context.SaveChanges();

                return Json(new { success = true, message = "Đặt hàng thành công." });
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " Các lỗi xác thực là: ", fullErrorMessage);

                // Log lỗi (bỏ chú thích dòng dưới để ghi vào file log hoặc cơ sở dữ liệu)
                // System.IO.File.WriteAllText(@"C:\ErrorLog.txt", exceptionMessage);

                return Json(new { success = false, message = exceptionMessage });
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi tiềm ẩn khác
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult SearchProduct(string productName)
        {
            try
            {
                var searchproducts = _context.Products
                                                 .Where(p => p.ProductName.Contains(productName))
                                                 .ToList();

                var model = new HomeViewModel
                {
                    ProductModels = searchproducts
                };

                return View("Product",model);
            }
            catch (Exception ex)
            {
                // Log the exception
                ViewBag.ErrorMessage = "An error occurred while fetching products.";
                return View("Error");
            }
        }
        public ActionResult Product(string categoryId)
        {
            try
            {
                var products = _context.Products.Include(p => p.Category)
                                                 .Where(p => p.CategoryID == categoryId)
                                                 .ToList();

                var model = new HomeViewModel
                {
                    ProductModels = products
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                ViewBag.ErrorMessage = "An error occurred while fetching products.";
                return View("Error");
            }
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
