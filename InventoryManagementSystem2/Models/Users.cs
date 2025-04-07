using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem2.Models
{
    public class Users
    {

        [Key]
            public int UserID { get; set; }
            public string FirstName { get; set; }
            public string LastName{ get; set; }
            public string Role { get; set; }

            [Required]
            public string UserName { get; set; }
            [Required]
            public string PassWord { get; set; }

    }
}
