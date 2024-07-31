using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TeaShop.ViewModel
{
    public class CreatePaymentViewModel
    {
        public List<SelectListItem> PaymentMethods { get; set; }
        public string BankCode { get; set; }
        public string Locale { get; set; }
    }
}