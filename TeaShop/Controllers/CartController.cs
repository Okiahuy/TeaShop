using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.Respository;

namespace TeaShop.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext db = new DataContext();

        // GET: Cart
        public ActionResult _CartPartial()
        {
            var customerId = (string)Session["CustomerID"];
            //lấy số lượng giỏ
            
            // Lấy danh sách giỏ hàng của người dùng đã đăng nhập
            var carts = db.Carts.Include("CartDetails.Product")
                                .Where(c => c.CustomerID == customerId)
                                .ToList();
            return PartialView(carts);
        }



        
        // POST: Cart/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int CartDetailID)
        {
            if (Session["CustomerID"] == null)
            {
                return Json(new { success = false, message = "Please login first." });
            }

            var cartDetail = db.CartDetails.Find(CartDetailID);
            if (cartDetail != null)
            {
                TempData["success"] = "Xóa sản phẩm thành công";
                db.CartDetails.Remove(cartDetail);
                db.SaveChanges();
            }
            else
            {
                TempData["success"] = "Không tìm thấy sản phẩm!";
            }

            var customerId = (string)Session["CustomerID"];
            var cart = db.Carts.Include("CartDetails")
                               .Where(c => c.CustomerID == customerId)
                               .ToList();
            int totalQuantity = db.Carts.Include("CartDetails")
                    .FirstOrDefault(c => c.CustomerID == customerId).CartDetails
                    .Count();
            Session["SoluongGio"] = totalQuantity;
            return RedirectToAction("Index", "Home");
            
        }


        // GET: Cart/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            return View();
        }

        
        // GET: Cart/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CartModel cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", cart.CustomerID);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditItem(int CartDetailID, string Size, int Quantity)
        {
            var cartDetail = db.CartDetails.Find(CartDetailID);
            if (cartDetail != null)
            {
                cartDetail.Size = Size;
                cartDetail.Quantity = Quantity;
                TempData["success"] = "Cập nhật giỏ hàng thành công!";
                db.SaveChanges();
                var customerId = (string)Session["CustomerID"];
                var cart = db.Carts.Include("CartDetails")
                                   .Where(c => c.CustomerID == customerId)
                                   .ToList();
                int totalQuantity = db.Carts.Include("CartDetails")
                    .FirstOrDefault(c => c.CustomerID == customerId).CartDetails
                    .Count();
                Session["SoluongGio"] = totalQuantity;
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("_CartPartial", "Product");
        }


        // GET: Cart/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CartModel cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }

            return View(cart);
        }


        // POST: Cart/AddToCart
/*        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(string productID, string size, int quantity)
        {
            var customerID = Session["CustomerID"].ToString();
            if (customerID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = db.Carts.FirstOrDefault(c => c.CustomerID == customerID);

            if (cart == null)
            {
                cart = new CartModel
                {
                    CustomerID = customerID,
                    CreatedDate = DateTime.Now
                };

                TempData["success"] = "Thêm giỏ hàng thành công";
                
                db.Carts.Add(cart);
                db.SaveChanges(); // Save to get the CartID

                int totalQuantity = db.Carts.Include("CartDetails")
                    .FirstOrDefault(c => c.CustomerID == customerID).CartDetails
                    .Count();
                Session["SoluongGio"] = totalQuantity;
            }

            var existingCartDetail = db.CartDetails
                .FirstOrDefault(cd => cd.CartID == cart.CartID && cd.ProductID == productID && cd.Size == size);

            if (existingCartDetail != null)
            {
                // Update quantity if the product already exists
                existingCartDetail.Quantity += quantity;
            }
            else
            {
                // Add cart detail
                var product = db.Products.Find(productID);
                if (product == null)
                {
                    return HttpNotFound();
                }

                var cartDetail = new CartDetailModel
                {
                    CartID = cart.CartID,
                    ProductID = productID,
                    Quantity = quantity,
                    Price = product.Price,
                    Size = size,
                    Image = product.ImageURL
                };
                TempData["success"] = "Thêm giỏ hàng thành công";
                db.CartDetails.Add(cartDetail);
                int totalQuantity = db.Carts.Include("CartDetails")
                    .FirstOrDefault(c => c.CustomerID == customerID).CartDetails
                    .Count();
                Session["SoluongGio"] = totalQuantity;
            }

            db.SaveChanges();
            return RedirectToAction("Product", "Product");
        }*/

       /* [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(string productID, string size, int quantity)
        {
            var customerID = Session["CustomerID"]?.ToString();
            if (customerID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = db.Carts.FirstOrDefault(c => c.CustomerID == customerID);
            if (cart == null)
            {
                cart = new CartModel
                {
                    CustomerID = customerID,
                    CreatedDate = DateTime.Now
                };
                db.Carts.Add(cart);
                db.SaveChanges(); // Save to get the CartID
                int totalQuantity = db.Carts.Include("CartDetails")
                    .FirstOrDefault(c => c.CustomerID == customerID).CartDetails
                    .Count();
                Session["SoluongGio"] = totalQuantity;
            }

            var existingCartDetail = db.CartDetails
                .FirstOrDefault(cd => cd.CartID == cart.CartID && cd.ProductID == productID && cd.Size == size);

            if (existingCartDetail != null)
            {
                // Update quantity if the product already exists
                existingCartDetail.Quantity += quantity;

            }
            else
            {
                // Add cart detail
                var product = db.Products.Find(productID);
                if (product == null || product.Stock < quantity)
                {
                    return Json(new { Notifications = $"Không đủ số lượng sản phẩm {product.ProductName}. Có sẵn: {product.Stock}, yêu cầu: {quantity}" });
                }
                var cartDetail = new CartDetailModel
                {
                    CartID = cart.CartID,
                    ProductID = productID,
                    Quantity = quantity,
                    Price = product.Price,
                    Size = size,
                    Image = product.ImageURL
                };

                db.CartDetails.Add(cartDetail);
                int totalQuantity = db.Carts.Include("CartDetails")
                    .FirstOrDefault(c => c.CustomerID == customerID).CartDetails
                    .Count();
                Session["SoluongGio"] = totalQuantity;
            }

            db.SaveChanges();
            return RedirectToAction("Product", "Product");
        }*/

        // POST: Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(string productID, string size, int quantity)
        {
            var customerID = Session["CustomerID"]?.ToString();
            if (string.IsNullOrEmpty(customerID))
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve or create a cart for the customer
            var cart = db.Carts.FirstOrDefault(c => c.CustomerID == customerID);
            if (cart == null)
            {
                cart = new CartModel
                {
                    CustomerID = customerID,
                    CreatedDate = DateTime.Now
                };
                TempData["success"] = "Thêm giỏ hàng thành công";
                db.Carts.Add(cart);
                db.SaveChanges(); // Save to get the CartID
                int totalQuantity = db.Carts.Include("CartDetails")
                   .FirstOrDefault(c => c.CustomerID == customerID).CartDetails
                   .Count();
                Session["SoluongGio"] = totalQuantity;
            }

            // Retrieve the product
            var product = db.Products.Find(productID);

            var discountPercentage = db.PromotionDetails
          .Where(pd => pd.ProductID == productID)
          .Select(pd => pd.Promotion.DiscountPercentage)
          .FirstOrDefault();

            var price = discountPercentage > 0
                ? product.Price * (1 - discountPercentage / 100m)
                : product.Price;

            // Calculate the price based on the applicable discount

            if (product == null)
            {
                return Json(new { Notifications = "Sản phẩm không tồn tại." });
            }

            // Check stock availability
            if (product.Stock < quantity)
            {
                return Json(new { Notifications = $"Không đủ số lượng sản phẩm {product.ProductName}. Có sẵn: {product.Stock}, yêu cầu: {quantity}" });
            }

            // Retrieve any applicable promotion


            // Check if the item is already in the cart
            var existingCartDetail = db.CartDetails
                .FirstOrDefault(cd => cd.CartID == cart.CartID && cd.ProductID == productID && cd.Size == size);

            if (existingCartDetail != null)
            {
                // Update the quantity if the item already exists
                existingCartDetail.Quantity += quantity;
            }
            else
            {

                // Add new cart detail
                var cartDetail = new CartDetailModel
                {
                    CartID = cart.CartID,
                    ProductID = productID,
                    Quantity = quantity,
                    Price = price,
                    Size = size,
                    Image = product.ImageURL
                };
                
                db.CartDetails.Add(cartDetail);
                int totalQuantity = db.Carts.Include("CartDetails")
                   .FirstOrDefault(c => c.CustomerID == customerID).CartDetails
                   .Count();
                Session["SoluongGio"] = totalQuantity;
            }

            db.SaveChanges();
            return RedirectToAction("Product", "Product");
        }

        public ActionResult PartialCart()
        {
            var customerID = Session["CustomerID"].ToString();
            if (customerID == null)
            {
                return PartialView("_CartPartial", new CartModel());
            }

            var cart = db.Carts.Include("CartDetails.Product").FirstOrDefault(c => c.CustomerID == customerID);
            return PartialView("_CartPartial", cart ?? new CartModel());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
}
    }
}