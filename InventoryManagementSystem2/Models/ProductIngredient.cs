using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem2.Models
{
    public class ProductIngredient
    {
        [Key]
        public int IngredientID { get; set; }
        public int ProductID { get; set; }
        public decimal Quantity { get; set; }

        // Navigation properties
        public virtual Ingredient Ingredient { get; set; }
        public virtual Product Product { get; set; }
    }

}
