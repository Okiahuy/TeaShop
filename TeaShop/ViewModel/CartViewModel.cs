using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeaShop.Models;

namespace TeaShop.ViewModel
{
    public class CartViewModel
    {
        public CartModel Cart { get; set; }
        public IEnumerable<CartDetailModel> CartDetails { get; set; }
        public IEnumerable<OrderModel> Orders { get; set; }
        public CartViewModel()
        {
            CartDetails = new List<CartDetailModel>();
        }
    }
}