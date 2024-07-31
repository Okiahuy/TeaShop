using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;


namespace TeaShop.Models
{
    [Table("Customers")]
    public class CustomerModel
    {
        [Key]
        [StringLength(255)]
        public string CustomerID { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; }

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        public virtual ICollection<OrderModel> Orders { get; set; }
        public virtual ICollection<CustomerReviewModel> CustomerReviews { get; set; }

        // Method to set the password using MD5
        public void SetPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                var hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                PasswordHash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Method to verify the password using MD5
        public bool VerifyPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                var hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hash == PasswordHash;
            }
        }
    }
}