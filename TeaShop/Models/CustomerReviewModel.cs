using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TeaShop.Models
{
    [Table("CustomerReviews")]
    public class CustomerReviewModel
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomerID { get; set; }

        [Required]
        [StringLength(255)]
        public string ProductID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        [ForeignKey("CustomerID")]
        public virtual CustomerModel Customer { get; set; }

        [ForeignKey("ProductID")]
        public virtual ProductModel Product { get; set; }
    }
}