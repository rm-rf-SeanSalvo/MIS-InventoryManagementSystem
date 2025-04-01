using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem2.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

}
