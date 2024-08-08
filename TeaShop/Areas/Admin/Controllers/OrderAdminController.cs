using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeaShop.Models;
using TeaShop.ViewModel;
using System.Data.Entity;
using TeaShop.Respository;
using Microsoft.Ajax.Utilities;
using PagedList;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.IO; // Thêm dòng này


namespace TeaShop.Areas.Admin.Controllers
{
    public class OrderAdminController : Controller
    {
        private readonly DataContext db = new DataContext();

        // GET: Admin/OrderAdmin


        public ActionResult Index(string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;

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
                CancellationReason = order.CancellationReason,
                OrderItems = order.OrderDetails.Select(od => new OrderItemViewModel
                {
                    ProductName = od.Product.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.Price
                }).ToList()
            }).ToList();

            int pageSize = 10; // Số lượng đơn hàng trên mỗi trang
            int pageNumber = (page ?? 1);

            var pagedOrderViewModels = orderViewModels.ToPagedList(pageNumber, pageSize);

            return View(pagedOrderViewModels);
        }


        //public ActionResult Index(string searchString)
        //{
        //    var orders = db.Orders.Include(o => o.Customer).ToList();
        //    var products = db.Products.ToList();
        //    ViewBag.ProductList = new SelectList(products, "ProductID", "ProductName");

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        orders = orders.Where(o => o.Customer.CustomerID.Contains(searchString)).ToList();
        //    }

        //    var orderViewModels = orders.Select(order => new OrderViewModel
        //    {
        //        OrderID = order.OrderID,
        //        CustomerName = order.Customer.CustomerName,
        //        Phone = order.Customer.Phone,
        //        Email = order.Customer.Email,
        //        Address = order.Customer.Address,
        //        OrderDate = order.OrderDate,
        //        TotalAmount = order.TotalAmount,
        //        OrderStatus = order.OrderStatus,
        //        CancellationReason = order.CancellationReason,
        //        OrderItems = order.OrderDetails.Select(od => new OrderItemViewModel
        //        {
        //            ProductID = od.ProductID,
        //            Quantity = od.Quantity,
        //            UnitPrice = od.Price
        //        }).ToList()
        //    }).ToList();

        //    return View(orderViewModels);
        //}

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
        public ActionResult UpdateStatus(int id, string orderStatus)
        {
            var order = db.Orders.Find(id);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CancelOrder(int id, string cancellationReason)
        {
            var order = db.Orders.Find(id);
            if (order != null)
            {
                order.OrderStatus = "Đã hủy";
                order.CancellationReason = cancellationReason;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int OrderID)
        {
            var order = db.Orders.Find(OrderID);
            if (order == null)
            {
                return HttpNotFound();
            }

            // Giả sử 3 là mã số của trạng thái "đã hủy"
            if (!order.OrderStatus.Equals("Đã hủy"))
            {
                TempData["err"]= "Chỉ xóa được đơn hàng có trạng thái hủy.";
            }
            TempData["success"] = "Xóa đơn hàng thành công!";
            db.Orders.Remove(order);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        //private string GenerateCustomerID()
        //{
        //    var latestCustomer = db.Customers.OrderByDescending(c => c.CustomerID).FirstOrDefault();
        //    if (latestCustomer == null)
        //    {
        //        return "KHTQ001";
        //    }

        //    string latestId = latestCustomer.CustomerID;
        //    int latestNumber;

        //    if (latestId.StartsWith("KHTQ") && latestId.Length == 7 && int.TryParse(latestId.Substring(4), out latestNumber))
        //    {
        //        int newNumber = latestNumber + 1;
        //        return "KHTQ" + newNumber.ToString("D3");
        //    }
        //    else
        //    {
        //        return "KHTQ001";
        //    }
        //}
        private string GenerateCustomerID()
        {
            string newCustomerID;
            bool isDuplicate;

            do
            {
                // Lọc các khách hàng có mã bắt đầu bằng "KHTQ"
                var customersWithKHTQ = db.Customers
                    .Where(c => c.CustomerID.StartsWith("KHTQ"))
                    .ToList();

                // Nếu không có khách hàng nào với mã bắt đầu bằng "KHTQ", đặt mã mới là "KHTQ001"
                if (!customersWithKHTQ.Any())
                {
                    newCustomerID = "KHTQ001";
                    isDuplicate = false;
                }
                else
                {
                    // Lọc và chuyển đổi các mã hợp lệ thành số
                    var validNumbers = customersWithKHTQ
                        .Select(c => c.CustomerID.Substring(4))  // Lấy phần số của mã
                        .Where(id => int.TryParse(id, out _))  // Lọc chỉ giữ các id có thể chuyển đổi thành số
                        .Select(int.Parse)  // Chuyển đổi thành số
                        .ToList();

                    // Nếu không có số hợp lệ, bắt đầu từ "KHTQ001"
                    if (!validNumbers.Any())
                    {
                        newCustomerID = "KHTQ001";
                        isDuplicate = false;
                    }
                    else
                    {
                        // Tìm mã lớn nhất hiện tại
                        int maxNumber = validNumbers.Max();

                        // Tạo mã mới với phần số tăng lên 1
                        int newNumber = maxNumber + 1;

                        // Tạo mã mới dưới dạng "KHTQ" + số mới với ba chữ số
                        newCustomerID = "KHTQ" + newNumber.ToString("D3");

                        // Kiểm tra xem mã mới có trùng lặp không
                        isDuplicate = db.Customers.Any(c => c.CustomerID == newCustomerID);
                    }
                }

            } while (isDuplicate);

            return newCustomerID;
        }

        public ActionResult PrintInvoice(int id)
        {
            using (var context = new DataContext())
            {
                // Truy vấn thông tin đơn hàng
                var orderDetails = context.OrderDetails
                    .Where(od => od.OrderID == id)
                    .Join(context.Orders,
                          od => od.OrderID,
                          o => o.OrderID,
                          (od, o) => new
                          {
                              o.OrderID,
                              o.CustomerID,
                              o.OrderDate,
                              o.TotalAmount,
                              o.OrderStatus,
                              o.CancellationReason,
                              od.OrderDetailID,
                              od.ProductID,
                              od.Quantity,
                              od.Size,
                              od.Price
                          })
                    .Join(context.Customers,
                          oo => oo.CustomerID,
                          c => c.CustomerID,
                          (oo, c) => new
                          {
                              oo.OrderID,
                              oo.OrderDate,
                              oo.TotalAmount,
                              oo.OrderStatus,
                              oo.CancellationReason,
                              oo.OrderDetailID,
                              oo.ProductID,
                              oo.Quantity,
                              oo.Size,
                              oo.Price,
                              c.CustomerName,
                              c.Address,
                              c.Phone
                          })
                    .Join(context.Products,
                          ooc => ooc.ProductID,
                          p => p.ProductID,
                          (ooc, p) => new
                          {
                              ooc.OrderID,
                              ooc.OrderDate,
                              ooc.TotalAmount,
                              ooc.OrderStatus,
                              ooc.CancellationReason,
                              ooc.OrderDetailID,
                              ooc.Quantity,
                              ooc.Size,
                              ooc.Price,
                              ooc.CustomerName,
                              ooc.Address,
                              ooc.Phone,
                              p.ProductName,
                              p.ImageURL,
                              Tongtien = ooc.Quantity * ooc.Price
                          })
                    .ToList();

                if (orderDetails.Any())
                {
                    // Sử dụng đường dẫn ảo cho các file font
                    string font1n = Server.MapPath("~/Areas/Admin/font/unifont/DejaVuSerifCondensed-Italic.ttf");
                    string nghiendam = Server.MapPath("~/Areas/Admin/font/unifont/DejaVuSerifCondensed-BoldItalic.ttf");
                    string fontPath = Server.MapPath("~/Areas/Admin/font/unifont/DejaVuSansCondensed-Bold.ttf");

                    if (!System.IO.File.Exists(font1n) || !System.IO.File.Exists(nghiendam) || !System.IO.File.Exists(fontPath))
                    {
                        Response.Write("<script>alert('Không tìm thấy file font!');</script>");
                        return new EmptyResult();
                    }

                    Document document = new Document();
                    MemoryStream memoryStream = new MemoryStream();
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Set up the font
                    BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font font = new Font(bf, 13, Font.NORMAL);
                    BaseFont bf4 = BaseFont.CreateFont(nghiendam, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font font4 = new Font(bf, 15, Font.NORMAL);
                    BaseFont bf2 = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font font2 = new Font(bf, 30, Font.NORMAL);
                    BaseFont bf1 = BaseFont.CreateFont(font1n, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font fontnghien = new Font(bf1, 15, Font.NORMAL);
                    LineSeparator line = new LineSeparator(1.0f, 100.0f, BaseColor.BLACK, Element.ALIGN_CENTER, 1);

                    // Thêm tiêu đề
                    Paragraph title = new Paragraph("HÓA ĐƠN", font2);
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);
                    document.Add(new Paragraph(" "));
                    document.Add(line);

                    // Tạo một đối tượng Paragraph để chứa mã đơn hàng và ngày đặt hàng
                    Paragraph codeAndDate = new Paragraph();
                    codeAndDate.Add(new Chunk("CODE: " + id, font));
                    codeAndDate.Add(Chunk.TABBING);
                    codeAndDate.Add(new Chunk("                                    CẢM ƠN BẠN ĐÃ MUA HÀNG! ", fontnghien));
                    document.Add(codeAndDate);
                    document.Add(new Paragraph(" "));
                    document.Add(line);

                    // Thêm thông tin đơn hàng
                    foreach (var item in orderDetails)
                    {
                        document.Add(new Paragraph("TÊN KHÁCH: " + item.CustomerName, font));
                        document.Add(new Paragraph("SẢN PHẨM: " + item.ProductName, font));
                        document.Add(new Paragraph("SỐ LƯỢNG: " + item.Quantity, font));
                        document.Add(new Paragraph("GIÁ: " + item.Price.ToString("C"), font)); // Assuming you want to format the price as currency
                        document.Add(new Paragraph("TỔNG TIỀN: " + item.Tongtien.ToString("C"), font)); // Assuming you want to format the total amount as currency
                        document.Add(new Paragraph("ĐỊA CHỈ GIAO: " + item.Address, font));
                        document.Add(new Paragraph("SỐ ĐIỆN THOẠI: " + item.Phone, font));
                        document.Add(new Paragraph("GHI CHÚ: " + item.CancellationReason, font));

                        // Thêm hình ảnh sản phẩm
                        string imagePath = Server.MapPath("~/Image/product.jpg"); // Đảm bảo đường dẫn đúng
                        if (System.IO.File.Exists(imagePath))
                        {
                            Image productImage = Image.GetInstance(imagePath);
                            productImage.ScaleToFit(100f, 100f); // Điều chỉnh kích thước hình ảnh

                            // Tạo đối tượng Paragraph để chứa hình ảnh và căn chỉnh bên phải
                            Paragraph imageParagraph = new Paragraph();
                            imageParagraph.Alignment = Element.ALIGN_RIGHT; // Căn chỉnh bên phải
                            imageParagraph.Add(productImage);
                            document.Add(imageParagraph);
                        }
                        else
                        {
                            document.Add(new Paragraph("Hình ảnh không tồn tại.", font));
                        }

                        document.Add(new Paragraph(" ")); // Empty line
                    }

                    document.Add(line);

                    Paragraph NGAYDAT = new Paragraph("Ngày lập: " + DateTime.Now.ToString("dd/MM/yyyy"), font4);
                    NGAYDAT.Alignment = Element.ALIGN_RIGHT;
                    document.Add(NGAYDAT);

                    // Đóng tài liệu
                    document.Close();

                    // Gửi file PDF đến trình duyệt
                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    Response.ContentType = "application/pdf";
                    Response.AppendHeader("Content-Disposition", "inline; filename=DONHANG_" + id + ".pdf");
                    Response.OutputStream.Write(bytes, 0, bytes.Length);
                    Response.OutputStream.Flush();
                    Response.OutputStream.Close();
                    return new EmptyResult();
                }
                else
                {
                    Response.Write("<script>alert('Không tìm thấy thông tin đơn hàng!');</script>");
                    return new EmptyResult();
                }
            }
        }




    }
}