namespace InventoryManagementSystem2.Models
{
    public class RecentProduct
    {
        public string ProductName { get; set; }
        public DateTime DateAdded { get; set; }
        public string QuantityWithUnit { get; set; } // e.g., "12kg"
    }

}
