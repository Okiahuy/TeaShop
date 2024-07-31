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
                db.CartDetails.Remove(cartDetail);
                db.SaveChanges();

                // Tính tổng tiền của các sản phẩm trong giỏ hàng
                var customerId = (string)Session["CustomerID"];
                var totalPrice = db.CartDetails.Where(cd => cd.Cart.CustomerID == customerId)
                                               .Sum(cd => cd.Quantity * cd.Price);

                return PartialView("Product", "Product");
            }

            return Json(new { success = false, message = "Cart detail not found." });
        }

        // GET: Cart/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            return View();
        }

        //// POST: Cart/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "CustomerID")] CartModel cart)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        cart.CreatedDate = DateTime.Now;
        //        db.Carts.Add(cart);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", cart.CustomerID);
        //    return View(cart);
        //}

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
                db.SaveChanges();
                return RedirectToAction("Product", "Product");
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
        [HttpPost]
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

                db.CartDetails.Add(cartDetail);
            }

            db.SaveChanges();
            return RedirectToAction("Product", "Product");
        }

        public ActionResult PartialCart()
        {
            var customerID = Session["CustomerID"]?.ToString();
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