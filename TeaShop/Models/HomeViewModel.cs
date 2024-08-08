
using TeaShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace TeaShop.Models
{
    public class HomeViewModel
    {
        public List<ProductModel> ProductModels { get; set; }
        public List<CustomerModel> CustomerModels { get; set; }
        public List<CartDetailModel> CartDetailModels { get; set; }
        public List<CartModel> CartModels { get; set; }

        public List<CategorieModel> CategoryList { get; set; }

        public List<PromotedProductViewModel> PromotedProducts { get; set; }

        public List<IngredientModel> Ingredients { get; set; }
    }

}

