using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;


namespace TeaShop.Models
{
    [Table("Categories")]
    public class CategorieModel
    {
        [Key]
        [StringLength(255)]
        public string CategoryID { get; set; }

        [Required]
        [StringLength(255)]
        public string CategoryName { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public virtual ICollection<ProductModel> Products { get; set; }
    }
}
