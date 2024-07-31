using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TeaShop.Models
{
    [Table("Positions")]
    public class PositionModel
    {
        [Key]
        [StringLength(255)]
        public string PositionID { get; set; }

        [Required]
        [StringLength(255)]
        public string PositionName { get; set; }

        public virtual ICollection<EmployeeModel> Employees { get; set; }
    }
}