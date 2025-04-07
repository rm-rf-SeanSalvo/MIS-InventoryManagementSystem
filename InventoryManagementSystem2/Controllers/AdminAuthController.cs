using InventoryManagementSystem2.DATA;
using InventoryManagementSystem2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

public class AdminAuthController : Controller
{
    private readonly InventoryManagementContext _context;

    public AdminAuthController(InventoryManagementContext context)
    {
        _context = context;
    }

    // GET: AdminAuth/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: AdminAuth/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Prepare the parameters for the stored procedure
        var userNameParam = new SqlParameter("@UserName", model.Username);
        var passwordParam = new SqlParameter("@Password", model.Password);

        // Call the stored procedure directly using FromSqlRaw
        var result = await _context.Users
            .FromSqlRaw("EXEC LoginAuthentication @UserName, @Password", userNameParam, passwordParam)
            .FirstOrDefaultAsync();

        // Check if the login is valid and redirect accordingly
        if (result != null && result.IsValid == 1)
        {
            TempData["Username"] = result.UserName;
            TempData["Role"] = result.Role;
            TempData["UserId"] = result.UserId;

            if (result.Role == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else if (result.Role == "Employee")
            {
                return RedirectToAction("EmployeeDashboard", "Employee");
            }
        }

        // If login fails
        ViewBag.Error = "Invalid username or password";
        return View(model);
    }
}
