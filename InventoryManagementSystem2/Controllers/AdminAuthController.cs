using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem2.Models;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http; 

namespace InventoryManagementSystem2.Controllers
{
    public class AuthenticatorController : Controller
    {
        private readonly IConfiguration _configuration;

        // Constructor to inject configuration
        public AuthenticatorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Login Page GET action
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login action POST (authenticate user using stored procedure)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await AuthenticateUser(model.Username, model.Password);

                if (result.IsValid)
                {
                    // If login is successful, you can store the user info in session or cookie.
                    // Here you can use the Role and UserId for further actions, like redirecting to different areas of the app.
                    // You could use a session variable for example:
                    HttpContext.Session.SetString("Role", result.Role);
                    HttpContext.Session.SetInt32("UserId", result.UserId);

                    // Redirect based on the role or to the requested URL
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we get here, something failed, redisplay the form.
            return View(model);
        }

        // Helper method to call the stored procedure and authenticate the user
        private async Task<(bool IsValid, string Role, int UserId)> AuthenticateUser(string username, string password)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            bool isValid = false;
            string role = null;
            int userId = 0;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("LoginAuthentication", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@UserName", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            isValid = reader.GetBoolean(reader.GetOrdinal("IsValid"));
                            if (isValid)
                            {
                                role = reader.GetString(reader.GetOrdinal("Role"));
                                userId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            }
                        }
                    }
                }
            }

            return (isValid, role, userId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
