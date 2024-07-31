using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TeaShop.Models
{
    [Table("EmployeeSales")]
    public class EmployeeSaleModel
    {
        [Key]
        public int SaleID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual EmployeeModel Employee { get; set; }

        [ForeignKey("OrderID")]
        public virtual OrderModel Order { get; set; }
    }
}