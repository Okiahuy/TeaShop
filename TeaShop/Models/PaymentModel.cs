using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeaShop.Models
{
    [Table("Payments")]
    public class PaymentModel
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public int PaymentMethodID { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [ForeignKey("OrderID")]
        public virtual OrderModel Order { get; set; }

        [ForeignKey("PaymentMethodID")]
        public virtual PaymentMethodModel PaymentMethod { get; set; }
    }
}