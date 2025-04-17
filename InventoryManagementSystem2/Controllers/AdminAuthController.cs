using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem2.Models;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace InventoryManagementSystem2.Controllers
{
    public class AdminAuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AdminAuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await AuthenticateUser(model.Username, model.Password);

                if (result.IsValid)
                {
                    HttpContext.Session.SetString("Role", result.Role);
                    HttpContext.Session.SetInt32("UserId", result.UserId);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Role, result.Role),
                        new Claim("UserId", result.UserId.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                    );

                    if (result.Role == "Manager")
                        return RedirectToAction("Index", "Home");
                    else if (result.Role == "Employee")
                        return RedirectToAction("Privacy", "Home");
                    else
                        return RedirectToAction("Login");
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            return View("~/Views/Home/Login.cshtml", model);
        }

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

        // ✅ Added GET method for RegisterUser
        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View("~/Views/Home/RegisterUser.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/RegisterUser.cshtml", model);

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var checkCmd = new SqlCommand("CheckExistingUser", connection))
                {
                    checkCmd.CommandType = CommandType.StoredProcedure;

                    checkCmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    checkCmd.Parameters.AddWithValue("@LastName", model.LastName);
                    var reader = await checkCmd.ExecuteReaderAsync();
                    int userCount = 0;

                    if (reader.Read())
                        userCount = reader.GetInt32(reader.GetOrdinal("UserCount"));

                    reader.Close();

                    if (userCount > 0)
                    {
                        ModelState.AddModelError(string.Empty, "User already exists.");
                        return View("~/Views/Home/RegisterUser.cshtml", model);
                    }
                }

                using (var insertCmd = new SqlCommand("RegisterUser", connection))
                {
                    insertCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    insertCmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    insertCmd.Parameters.AddWithValue("@LastName", model.LastName);
                    insertCmd.Parameters.AddWithValue("@Password", model.Password);
                    insertCmd.Parameters.AddWithValue("@Role", model.Role);

                    await insertCmd.ExecuteNonQueryAsync();
                }
            }

            TempData["SuccessMessage"] = "Account created successfully. Please log in.";
            return RedirectToAction("Login");
        }
    }
}
