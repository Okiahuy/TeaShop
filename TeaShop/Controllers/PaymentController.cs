using TeaShop.Models;
using TeaShop.Respository;
using TeaShop.ViewModel;
using TeaShop.VNPay;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;



namespace TeaShop.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DataContext _context = new DataContext();

        // Action to display the payment creation form
        public ActionResult CreatePayment()
        {
            var paymentMethods = _context.PaymentMethods.ToList();
            var model = new CreatePaymentViewModel
            {
                PaymentMethods = paymentMethods.Select(pm => new SelectListItem
                {
                    Value = pm.PaymentMethodID.ToString(),
                    Text = pm.MethodName
                }).ToList()
            };

            return View(model);
        }

        // Action to create the payment URL
        [HttpPost]
        public ActionResult CreatePaymentUrl(CreatePaymentViewModel model)
        {
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string vnp_ReturnUrl = Url.Action("PaymentResult", "Payment", null, Request.Url.Scheme);

            // Create a dummy order for payment
            OrderModel order = new OrderModel
            {
                OrderID = (int)(DateTime.Now.Ticks % int.MaxValue),
                TotalAmount = 100000,
                OrderStatus = "0",
                OrderDate = DateTime.Now
            };

            // Save order to db
            // Add your code here to save order to your database

            // Build URL for VNPAY
            VNPayLib vnpay = new VNPayLib();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.TotalAmount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", order.OrderDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);

            if (model.BankCode == "VNPAYQR")
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (model.BankCode == "VNBANK")
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (model.BankCode == "INTCARD")
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderID);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderID.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return Redirect(paymentUrl);
        }

        // Action to handle the payment result from VNPay
        public ActionResult PaymentResult()
        {
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            VNPayLib pay = new VNPayLib();
            NameValueCollection vnpayData = Request.QueryString;

            foreach (string s in vnpayData)
            {
                if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                {
                    pay.AddResponseData(s, vnpayData[s]);
                }
            }

            bool checkSignature = pay.ValidateSignature(vnpayData["vnp_SecureHash"], vnp_HashSecret);

            if (checkSignature)
            {
                string vnp_ResponseCode = vnpayData["vnp_ResponseCode"];
                if (vnp_ResponseCode == "00")
                {
                    // Save payment information to database
                    var payment = new PaymentModel
                    {
                        OrderID = Convert.ToInt32(vnpayData["vnp_OrderInfo"]),
                        PaymentMethodID = Convert.ToInt32(vnpayData["vnp_PaymentMethodID"]),
                        PaymentDate = DateTime.Now,
                        Amount = Convert.ToDecimal(vnpayData["vnp_Amount"]) / 100,
                    };

                    _context.Payments.Add(payment);
                    _context.SaveChanges();

                    ViewBag.Message = "Thanh toán thành công";
                }
                else
                {
                    ViewBag.Message = "Thanh toán không thành công";
                }
            }
            else
            {
                ViewBag.Message = "Chữ ký không hợp lệ";
            }

            return View();
        }
    }
}
