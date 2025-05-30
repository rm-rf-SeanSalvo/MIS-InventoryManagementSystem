namespace InventoryManagementSystem2.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalCategories { get; set; }
        public int TotalProducts { get; set; }

        public List<RecentProduct> RecentProducts { get; set; }
        public List<LowStock> LowStockItems { get; set; }

        public DashboardViewModel()
        {
            RecentProducts = new List<RecentProduct>();
            LowStockItems = new List<LowStock>();
        }
    }

}
