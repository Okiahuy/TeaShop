using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeaShop.Models
{
    [Table("EmployeeSalarys")]
    public class EmployeeSalaryModel
    {

        [Key]
        public int SalaryID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int TotalSales { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SalaryAmount { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual EmployeeModel Employee { get; set; }
    }
}