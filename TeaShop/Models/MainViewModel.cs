using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeaShop.Models
{
    public class MainViewModel
    {
        public IEnumerable<CategorieModel> CategoryList { get; set; }
    }
}