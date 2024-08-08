using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeaShop.ViewModel
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string CancellationReason { get; set; }
        public bool IsLatestOrder { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}