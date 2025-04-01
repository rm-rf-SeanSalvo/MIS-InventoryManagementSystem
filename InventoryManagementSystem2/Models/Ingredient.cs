using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem2.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientID { get; set; }
        public string Name { get; set; }
        public string UnitOfMeasure { get; set; }
    }

}
