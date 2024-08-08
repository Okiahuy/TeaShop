using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;


namespace TeaShop.Models
{
    [Table("Orders")]
    public class OrderModel
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomerID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderStatus { get; set; }
        //Mới cập nhật
        public string CancellationReason { get; set; } = string.Empty;

        [ForeignKey("CustomerID")]
        public virtual CustomerModel Customer { get; set; }

        public virtual ICollection<OrderDetailModel> OrderDetails { get; set; }
        public virtual ICollection<EmployeeSaleModel> EmployeeSales { get; set; }
        public virtual ICollection<PaymentModel> Payments { get; set; }
    }
}