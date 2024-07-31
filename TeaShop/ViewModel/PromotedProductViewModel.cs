using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeaShop.ViewModel
{
    public class PromotedProductViewModel
    {
        public string ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime EndDate { get; set; }
        public string ImageURL { get; set; }
    }
}