using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem2.Models;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace InventoryManagementSystem2.Controllers
{
    public class AdminAuthController : Controller
    {
        private readonly IConfiguration _configuration;

        // Constructor to inject configuration
        public AdminAuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Login Page GET action
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        // POST action that handles the login logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await AuthenticateUser(model.Username, model.Password);

                if (result.IsValid)
                {
                    // Handle user session and authentication logic
                    HttpContext.Session.SetString("Role", result.Role);
                    HttpContext.Session.SetInt32("UserId", result.UserId);

                    // Create claims for the user
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, result.Role),
                new Claim("UserId", result.UserId.ToString())
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Sign in the user
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                    );

                    // Redirect based on the user's role
                    if (result.Role == "Manager")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.Role == "Employee")
                    {
                        return RedirectToAction("Privacy", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

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
