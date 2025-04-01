using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem2.Models
{
    public class Users
    {
    [Key]
        public int UserID { get; set; }
        public string FN { get; set; }
        public string LN { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PW { get; set; }
    }
}
