using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace TeaShop.Models
{
    [Table("PaymentMethods")]
    public class PaymentMethodModel
    {
        [Key]
        public int PaymentMethodID { get; set; }

        [Required]
        [StringLength(255)]
        public string MethodName { get; set; }

        public virtual ICollection<PaymentModel> Payments { get; set; }
    }
}