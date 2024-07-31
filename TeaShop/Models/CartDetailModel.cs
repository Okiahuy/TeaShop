using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TeaShop.Models
{
    [Table("CartDetails")]
    public class CartDetailModel
    {
        [Key]
        public int CartDetailID { get; set; }

        [Required]
        public int CartID { get; set; }

        [Required]
        [StringLength(255)]
        public string ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [ForeignKey("CartID")]
        public virtual CartModel Cart { get; set; }

        [ForeignKey("ProductID")]
        public virtual ProductModel Product { get; set; }

        [Required]
        [StringLength(50)]
        public string Size { get; set; }

        [StringLength(255)]
        public string Image { get; set; }
    }
}