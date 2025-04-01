using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem2.Models
{
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int UserID { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderStatus { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual Users User { get; set; }

    }

}
