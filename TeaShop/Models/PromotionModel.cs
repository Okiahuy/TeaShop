using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TeaShop.Models
{
    [Table("Promotions")]
    public class PromotionModel
    {
        [Key]
        [StringLength(255)]
        public string PromotionID { get; set; }

        [Required]
        [StringLength(255)]
        public string PromotionName { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public virtual ICollection<PromotionDetailModel> PromotionDetails { get; set; }
    }
}