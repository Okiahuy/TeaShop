using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeaShop.Respository;

namespace TeaShop.Controllers
{
    public class OrderController : Controller
    {

        private readonly DataContext db = new DataContext();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int OrderID, string CancellationReason)
        {
            var order = db.Orders.Find(OrderID);
            if (order != null && order.OrderStatus == "Chờ xử lý")
            {
                order.OrderStatus = "Đã hủy";
                order.CancellationReason = CancellationReason;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                // Có thể thêm logic để thông báo cho admin tại đây
            }
            return RedirectToAction("OrderConfirmation", "OrderDetail", new { id = OrderID });
        }

        // Các action khác của OrderController
    }
}