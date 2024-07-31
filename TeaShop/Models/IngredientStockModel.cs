using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeaShop.Models
{
    [Table("IngredientStocks")]
    public class IngredientStockModel
    {
        [Key]
        public int StockID { get; set; }

        [Required]
        [StringLength(255)]
        public string IngredientID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Quantity { get; set; }

        [Required]
        public DateTime StockDate { get; set; }

        [ForeignKey("IngredientID")]
        public virtual IngredientModel Ingredient { get; set; }
    }
}