using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TeaShop.Models
{
    [Table("Employees")]
    public class EmployeeModel
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(255)]
        public string EmployeeName { get; set; }

        [StringLength(255)]
        public string PositionID { get; set; }

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [ForeignKey("PositionID")]
        public virtual PositionModel Position { get; set; }

        public virtual ICollection<EmployeeSaleModel> EmployeeSales { get; set; }
        public virtual ICollection<EmployeeSalaryModel> EmployeeSalaries { get; set; }
    }
}