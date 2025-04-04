using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace InventoryManagementSystem2.Models
{
    public class Customers
    {
        [Key]
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ContactNumber { get; set; }
        [Required]
        public string Email { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
    }

}
