namespace InventoryManagementSystem2.Models
{
    public class LowStock
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; } // or string if you want unit as well
        public string StockStatus { get; set; }
    }
}
