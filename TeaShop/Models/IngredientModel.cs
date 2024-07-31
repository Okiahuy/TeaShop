using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TeaShop.Models
{
    [Table("Ingredients")]
    public class IngredientModel
    {
        [Key]
        [StringLength(255)]
        public string IngredientID { get; set; }

        [Required]
        [StringLength(255)]
        public string IngredientName { get; set; }

        [StringLength(255)]
        public string SupplierID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [ForeignKey("SupplierID")]
        public virtual SupplierModel Supplier { get; set; }

        public virtual ICollection<RecipiModel> Recipes { get; set; }
        public virtual ICollection<IngredientStockModel> IngredientStocks { get; set; }
    }
}