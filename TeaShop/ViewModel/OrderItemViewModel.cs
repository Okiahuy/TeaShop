using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeaShop.ViewModel
{
    public class OrderItemViewModel
    {
        public int OrderID { get; set; }

        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Size { get; set; }
    }
}