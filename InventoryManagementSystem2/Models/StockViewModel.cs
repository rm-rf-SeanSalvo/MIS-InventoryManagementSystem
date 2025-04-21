using System;

namespace InventoryManagementSystem2.Models
{
    public class StockViewModel
    {
        public int IngredientID { get; set; }
        public string IngredientName { get; set; }
        public string CategoryName { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal InStock { get; set; }
        public DateTime LastReplenished { get; set; }

        public Ingredient Ingredient { get; set; }
        public Category Category { get; set; }
    }
}
