using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeaShop.ViewModel
{
    public class StatisticsViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public int TotalCustomers { get; set; }
        public int InStoreCustomers { get; set; }
        public int OnlineCustomers { get; set; }
        public decimal NetRevenue { get; set; }
    }
}