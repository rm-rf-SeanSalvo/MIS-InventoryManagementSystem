namespace InventoryManagementSystem2.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalCategories { get; set; }
        public int TotalProducts { get; set; }
        public List<RecentIngredientViewModel> RecentIngredients { get; set; } = new();

    }

    public class RecentIngredientViewModel
    {
        public string IngredientName { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedByUsername { get; set; }
    }

}
