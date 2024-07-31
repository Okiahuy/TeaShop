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

                TempData["success"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu không đúng.");
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
                CustomerID = GenerateCustomerID(),
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
        private string GenerateCustomerID()
        {
            var latestCustomer = _context.Customers.OrderByDescending(c => c.CustomerID).FirstOrDefault();
            if (latestCustomer == null)
            {
                return "KH001";
            }

            string latestId = latestCustomer.CustomerID;
            int latestNumber = int.Parse(latestId.Substring(2));
            int newNumber = latestNumber + 1;

            return "KH" + newNumber.ToString("D3");
        }
    }
}
