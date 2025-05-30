using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem2.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string UnitOfMeasure { get; set; }

        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category Category { get; set; }

        public decimal Quantity { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime? LastReplenished { get; set; }  // Add this
    }
}
