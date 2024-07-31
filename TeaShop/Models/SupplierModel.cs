using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TeaShop.Models
{
    [Table("Suppliers")]
    public class SupplierModel
    {
        [Key]
        [StringLength(255)]
        public string SupplierID { get; set; }

        [Required]
        [StringLength(255)]
        public string SupplierName { get; set; }

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        public virtual ICollection<IngredientModel> Ingredients { get; set; }
    }
}