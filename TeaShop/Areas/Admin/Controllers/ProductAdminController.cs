﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TeaShop.Models;
using TeaShop.Respository;
using PagedList;

namespace TeaShop.Areas.Admin.Controllers
{
    public class ProductAdminController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Admin/ProductAdmin/Product
        // Add this using statement

        public ActionResult Product(int? page, string searchProductName)
        {
            int pageNumber = page ?? 1; // Mặc định trang 1 nếu page null
            int pageSize = 10; // Số mục trên mỗi trang

            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchProductName))
            {
                products = products.Where(p => p.ProductName.Contains(searchProductName));
            }

            // Sắp xếp dữ liệu trước khi phân trang
            products = products.OrderBy(p => p.ProductID);

            var pagedProducts = products.ToPagedList(pageNumber, pageSize);

            ViewBag.CurrentFilter = searchProductName;

            var categories = db.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName");

            return View(pagedProducts);
        }

        // POST: Admin/ProductAdmin/Product
        [HttpPost]
        public ActionResult Product(ProductModel model, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        string folderPath = Server.MapPath("~/Images");
                        string fileName = Path.GetFileName(ImageFile.FileName);
                        string fileExtension = Path.GetExtension(fileName);
                        string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
                        string[] allowedMimeTypes = { "image/png", "image/jpeg" };
                        var subdirectories = Directory.GetDirectories(folderPath);
                        // Kiểm tra phần mở rộng tệp
                        if (allowedExtensions.Contains(fileExtension))
                        {
                            // Kiểm tra loại MIME của tệp
                            var mimeType = ImageFile.ContentType;
                            if (allowedMimeTypes.Contains(mimeType))
                            {
                                // Đảm bảo tên tệp là duy nhất để tránh ghi đè lên tệp hiện có
                                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                                string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                                ImageFile.SaveAs(path);
                                model.ImageURL = fileName;
                            }
                            else
                            {
                                ModelState.AddModelError("ImageFile", "Loại tệp không hợp lệ. Chỉ chấp nhận tệp ảnh định dạng .png hoặc .jpg.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("ImageFile", "Phần mở rộng tệp không hợp lệ. Chỉ chấp nhận tệp ảnh định dạng .png hoặc .jpg.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImageFile", "Vui lòng chọn một tệp ảnh.");
                    }



                    // Generating a unique ProductID
                    model.ProductID = GenerateProductID();
                    TempData["success"] = "Thêm sản phẩm thành công";
                    db.Products.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Product", "ProductAdmin");
                }

                var products = db.Products.ToList();
                return View(products); // Return to the list view with the products and validation errors
            }
            catch (Exception ex)
            {
                // Log the error (uncomment ex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                var products = db.Products.ToList();
                return View(products);
            }
        }



        private string GenerateProductID()
        {
            // Logic to generate a unique ProductID
            var lastProduct = db.Products.OrderByDescending(p => p.ProductID).FirstOrDefault();
            if (lastProduct != null)
            {
                int lastID = int.Parse(lastProduct.ProductID.Substring(3)); // Adjust the substring as needed
                return "PRD" + (lastID + 1).ToString("D3");
            }
            else
            {
                return "PRD001";
            }
        }

        public ActionResult Edit(int id)
        {
            ProductModel product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy sản phẩm
            }

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductModel product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = db.Products.Find(product.ProductID);

                    if (existingProduct != null)
                    {
                        existingProduct.ProductName = product.ProductName;
                        existingProduct.CategoryID = product.CategoryID;
                        existingProduct.Price = product.Price;
                        existingProduct.Stock = product.Stock;
                        existingProduct.Description = product.Description;
                        string imagePathInOthers = Server.MapPath("~/Images/Others/" + product.ImageURL);
                        string imagePathInImages = Server.MapPath("~/Images/" + product.ImageURL);

                        // Cập nhật ViewBag để truyền đường dẫn hình ảnh
                        ViewBag.ImageURL = System.IO.File.Exists(imagePathInOthers) ?
                            Url.Content("~/Images/Others/" + product.ImageURL) :
                            Url.Content("~/Images/" + product.ImageURL);
                        // Xử lý cập nhật hình ảnh nếu có tệp hình ảnh mới được tải lên
                        if (ImageFile != null && ImageFile.ContentLength > 0)
                        {

                            string fileName = Path.GetFileName(ImageFile.FileName);

                            string uniqueFileName = fileName;
                            string categoryFolder = "Others"; // Thay đổi thành thư mục "Others"

                            string path = Path.Combine(Server.MapPath($"~/Images/{categoryFolder}"), uniqueFileName);
                            Directory.CreateDirectory(Path.GetDirectoryName(path)); // Tạo thư mục nếu chưa tồn tại
                            ImageFile.SaveAs(path);
                            existingProduct.ImageURL = Path.Combine(categoryFolder, uniqueFileName); // Cập nhật đường dẫn tương đối
                        }
                        else
                        {
                            ModelState.AddModelError("ImageFile", "Loại tệp không hợp lệ. Chỉ chấp nhận tệp ảnh định dạng .png hoặc .jpg.");
                            return View(product);
                        }
                        TempData["success"] = "Sửa sản phẩm thành công";
                        db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                        return RedirectToAction("Product", "ProductAdmin"); // Chuyển hướng về trang quản lý sản phẩm sau khi cập nhật thành công
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi xảy ra khi cập nhật sản phẩm: " + ex.Message);
                }
            }

            // Nếu ModelState không hợp lệ, hiển thị lại form với thông tin nhập liệu và thông báo lỗi
            return View(product);
        }

        public string GetCategoryFolder(string categoryID)
        {
            switch (categoryID)
            {
                case "CAT004":
                    return "NuocEp";
                case "CAT002":
                    return "TraSua";
                case "CAT005":
                    return "TraTraiCay";
                case "CAT003":
                    return "Cafe";
                case "CAT001":
                    return "BanhNgot";
                default:
                    return "Others"; // Thư mục mặc định nếu không khớp với danh mục nào
            }
        }

        // GET: Admin/ProductAdmin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string ProductID)
        {

            ProductModel product = db.Products.Find(ProductID);
            if (product == null)
            {
                return HttpNotFound();
            }
            TempData["success"] = "Xóa sản phẩm thành công";
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Product", "ProductAdmin");
        }

        [HttpGet]
        public ActionResult Searchs(string ProductName)
        {
            // Fetch products based on the search criteria
            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(ProductName))
            {
                products = products.Where(p => p.ProductName.Contains(ProductName));
            }

            // Populate ViewBag.Categories
            var categories = db.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName");

            return View("Product", products.ToList());
        }



    }

}