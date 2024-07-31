using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TeaShop.Models
{
    [Table("PromotionDetails")]
    public class PromotionDetailModel
    {
        [Key]
        public int PromotionDetailID { get; set; }

        [Required]
        [StringLength(255)]
        public string PromotionID { get; set; }

        [Required]
        [StringLength(255)]
        public string ProductID { get; set; }

        [ForeignKey("PromotionID")]
        public virtual PromotionModel Promotion { get; set; }

        [ForeignKey("ProductID")]
        public virtual ProductModel Product { get; set; }
    }
}