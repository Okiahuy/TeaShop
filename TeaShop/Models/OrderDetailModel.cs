using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TeaShop.Models
{
    [Table("OrderDetails")]
    public class OrderDetailModel
    {
        [Key]
        public int OrderDetailID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        [StringLength(255)]
        public string ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [ForeignKey("OrderID")]
        public virtual OrderModel Order { get; set; }

        [ForeignKey("ProductID")]
        public virtual ProductModel Product { get; set; }

    }
}