using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InventoryManagementSystem2.DATA;
using Microsoft.AspNetCore.Mvc;

public class DatabaseController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseController> _logger;

    public DatabaseController(AppDbContext context, ILogger<DatabaseController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult TestConnection()
    {
        try
        {

            _context.Database.OpenConnection();
            _logger.LogInformation("Database connection successful!");
            return Ok("Database connection successful!");

        }
        catch (Exception ex)
        {
            _logger.LogError($"Database connection failed: {ex.Message}");
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}
