using InventoryManagementSystem2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    // This action is for your login page
    public IActionResult Login()
    {
        return View();
    }
    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            RecentIngredients = new List<RecentIngredientViewModel>()
        };

        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Total Users
            using (var cmd = new SqlCommand("GetTotalUsers", connection) { CommandType = CommandType.StoredProcedure })
            {
                var result = await cmd.ExecuteScalarAsync();
                model.TotalUsers = int.TryParse(result?.ToString(), out int totalUsers) ? totalUsers : 0;
            }

            // Total Categories
            using (var cmd = new SqlCommand("GetTotalCategories", connection) { CommandType = CommandType.StoredProcedure })
            {
                var result = await cmd.ExecuteScalarAsync();
                model.TotalCategories = result != null ? Convert.ToInt32(result) : 0;
            }

            // Total Products
            using (var cmd = new SqlCommand("GetTotalProducts", connection) { CommandType = CommandType.StoredProcedure })
            {
                var result = await cmd.ExecuteScalarAsync();
                model.TotalProducts = result != null ? Convert.ToInt32(result) : 0;
            }

            // Recently Modified Ingredients
            using (var cmd = new SqlCommand("GetRecentlyModifiedIngredients", connection) { CommandType = CommandType.StoredProcedure })
            {
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    model.RecentIngredients.Add(new RecentIngredientViewModel
                    {
                        IngredientName = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                        ModifiedDate = reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1),
                        ModifiedByUsername = reader.IsDBNull(2) ? "Unknown" : reader.GetString(2)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading dashboard: " + ex.Message;

            // Defensive fallback in case of error
            model.TotalUsers = 0;
            model.TotalCategories = 0;
            model.TotalProducts = 0;
            model.RecentIngredients = new List<RecentIngredientViewModel>();
        }

        return View("~/Views/Home/Index.cshtml", model);
    }

    public IActionResult Privacy()
    {
        return View(); // Employee's Privacy page
    }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
