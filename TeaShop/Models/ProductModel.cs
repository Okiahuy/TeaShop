using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeaShop.Models
{
    [Table("Products")]
    public class ProductModel
    {
        [Key]
        [StringLength(255)]
        public string ProductID { get; set; }

        [Required]
        [StringLength(255)]
        public string ProductName { get; set; }

        [StringLength(255)]
        public string CategoryID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        public string Description { get; set; }

        [StringLength(255)]
        public string ImageURL { get; set; }

        [ForeignKey("CategoryID")]
        public virtual CategorieModel Category { get; set; }

        public virtual ICollection<OrderDetailModel> OrderDetails { get; set; }
        public virtual ICollection<PromotionDetailModel> PromotionDetails { get; set; }
        public virtual ICollection<CustomerReviewModel> CustomerReviews { get; set; }
        public virtual ICollection<RecipiModel> Recipes { get; set; }
        public virtual ICollection<CartDetailModel> CartDetails { get; set; }
    }
}