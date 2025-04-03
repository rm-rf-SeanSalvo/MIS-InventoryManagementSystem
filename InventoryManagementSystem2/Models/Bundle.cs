using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem2.Models
{
    public class Bundle
    {
        [Key]
        public int BundleID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }   
        public decimal Price { get; set; }
        //
        public virtual Product Product { get; set; }
    }

}
