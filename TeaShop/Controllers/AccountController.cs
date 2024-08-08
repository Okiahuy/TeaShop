using TeaShop.Models;  // Assuming TeaShop.Models contains the model classes
using TeaShop.Respository;  // Assuming TeaShop.Respository contains the DataContext
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TeaShop.ViewModel;
 // Assuming TeaShop.ViewModel contains view models

namespace TeaShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context = new DataContext();

        // Hàm Đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // Hàm đăng kí
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // Hàm đăng xuất
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // Hàm băm mật khẩu
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashedBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        // Hàm xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var hashpass = HashPassword(model.Password);
            var user = await _context.Customers.FirstOrDefaultAsync(c =>
                c.Username == model.Username && c.PasswordHash == hashpass);

            if (user != null)
            {
                Session["CustomerID"] = user.CustomerID;
                Session["Username"] = user.Username;
                Session["CustomerName"] = user.CustomerName;
                Session["Phone"] = user.Phone;
                Session["Email"] = user.Email;
                Session["Address"] = user.Address;
                //số lượng giỏ hàng
                string accountId = Session["CustomerID"].ToString();

                var cart = await _context.Carts.Include("CartDetails")
                                    .FirstOrDefaultAsync(c => c.CustomerID == accountId);
                if (cart == null || cart.CartDetails == null || !cart.CartDetails.Any())
                {
                    Session["SoluongGio"] = 0;
                }
                else
                {
                    int totalQuantity = cart.CartDetails.Count();
                    Session["SoluongGio"] = totalQuantity;
                }

                TempData["success"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu không đúng.");
            TempData["err"] = "Đăng nhập thất bại!";
            return View(model);
        }


        // Hàm đăng kí
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(CustomerModel customerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(customerModel);
            }

            var checkUsername = await _context.Customers.AnyAsync(c => c.Username == customerModel.Username);
            if (checkUsername)
            {
                TempData["err"] = "Tên tài khoản đã tồn tại. Vui lòng chọn tên tài khoản khác.";
                return View(customerModel);
            }

            var ListInforCustomer = new CustomerModel
            {
                CustomerID = GenerateCustomerIDForUser(),
                CustomerName = customerModel.CustomerName,
                Email = customerModel.Email,
                Phone = customerModel.Phone,
                Address = customerModel.Address,
                Username = customerModel.Username,
                PasswordHash = HashPassword(customerModel.PasswordHash)
            };

            _context.Customers.Add(ListInforCustomer);
            await _context.SaveChangesAsync();

            TempData["success"] = "Đăng ký thành công. Bạn có thể đăng nhập ngay bây giờ.";
            return RedirectToAction("Login", "Account");
        }

        // Hàm sinh mã tự động
        private string GenerateCustomerIDForUser()
        {
            // Lọc các khách hàng có mã bắt đầu bằng "KH"
            var customersWithKH = _context.Customers
                .Where(c => c.CustomerID.StartsWith("KH"))
                .ToList();

            // Nếu không có khách hàng nào với mã bắt đầu bằng "KH", đặt mã mới là "KH001"
            if (!customersWithKH.Any())
            {
                return "KH001";
            }

            // Lọc và chuyển đổi các mã hợp lệ thành số
            var validNumbers = customersWithKH
                .Select(c => c.CustomerID.Substring(2))
                .Where(id => int.TryParse(id, out _))  // Lọc chỉ giữ các id có thể chuyển đổi thành số
                .Select(int.Parse)  // Chuyển đổi thành số
                .ToList();

            // Nếu không có số hợp lệ, bắt đầu từ "KH001"
            if (!validNumbers.Any())
            {
                return "KH001";
            }

            // Tìm mã lớn nhất hiện tại
            int maxNumber = validNumbers.Max();

            // Tạo mã mới với phần số tăng lên 1
            int newNumber = maxNumber + 1;
            return "KH" + newNumber.ToString("D3");
        }




    }
}