using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TeaShop.Models
{
    [Table("Carts")]
    public class CartModel
    {
        [Key]
        public int CartID { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomerID { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [ForeignKey("CustomerID")]
        public virtual CustomerModel Customer { get; set; }

        public virtual ICollection<CartDetailModel> CartDetails { get; set; }



        //[StringLength(255)]
        //public string ImageURL { get; set; }

        //[Required]
        //[StringLength(255)]
        //public string ProductName { get; set; }
    }
}