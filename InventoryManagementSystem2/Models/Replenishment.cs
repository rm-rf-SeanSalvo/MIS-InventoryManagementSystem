using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem2.Models
{
    public class Replenishment
    {
        [Key]
        public int ReplenishmentID { get; set; }
        public int IngredientID { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public int UserID { get; set; }

        // Navigation properties
        public virtual Ingredient Ingredient { get; set; }
        public virtual User User { get; set; }
    }

}
