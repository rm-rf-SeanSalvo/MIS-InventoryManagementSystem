namespace InventoryManagementSystem2.Models
{
    public class LoginResult
    {
        public int IsValid { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
