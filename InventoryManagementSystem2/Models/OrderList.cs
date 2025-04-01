using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace InventoryManagementSystem2.Models
{
    public class OrderList
    {
        [Key]
        public int OrderListID { get; set; }
        public int OrderID { get; set; }
        public int? ProductID { get; set; }
        public int? BundleID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }

        // Navigation properties
        public virtual Orders Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual Bundle Bundle { get; set; }
    }

}
