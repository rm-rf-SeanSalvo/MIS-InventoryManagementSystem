using InventoryManagementSystem2.DATA;
using InventoryManagementSystem2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace InventoryManagementSystem2.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly InventoryManagementContext _context;

        public AdminController(IConfiguration configuration, InventoryManagementContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //var model = new DashboardViewModel();

            //// Init collections (if not already done in class)
            //model.LowStockItems = new List<LowStock>();
            //model.RecentProducts = new List<RecentProduct>();

            //using (var conn = _context.Database.GetDbConnection())
            //{
            //    await conn.OpenAsync();

            //    using (var cmd = conn.CreateCommand())
            //    {
            //        cmd.CommandText = "GetTotalUsers";
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        model.TotalUsers = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            //        cmd.CommandText = "GetTotalCategories";
            //        model.TotalCategories = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            //        cmd.CommandText = "GetTotalProducts";
            //        model.TotalProducts = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            //    }

            //    using (var cmd = conn.CreateCommand())
            //    {
            //        cmd.CommandText = "GetLowStockItems";
            //        cmd.CommandType = CommandType.StoredProcedure;

            //        using (var reader = await cmd.ExecuteReaderAsync())
            //        {
            //            while (await reader.ReadAsync())
            //            {
            //                model.LowStockItems.Add(new LowStock
            //                {
            //                    ProductName = reader["ProductName"].ToString(),
            //                    Quantity = 0 // or read real value if needed
            //                });
            //            }
            //        }
            //    }
            //}


            Console.WriteLine("HIT ADMIN INDEX");
            var model = new DashboardViewModel();
            model.LowStockItems = new List<LowStock>();
            model.RecentProducts = new List<RecentProduct>();

            return View("~/Views/Home/Index.cshtml", model);
        }


        public async Task<IActionResult> Employees()
        {
            var employees = new List<EmployeeViewModel>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("ViewEmployees", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employees.Add(new EmployeeViewModel
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        Role = reader.GetString(reader.GetOrdinal("Role")),
                        Status = reader.GetBoolean(reader.GetOrdinal("IsActive")) ? "Active" : "Inactive",
                        LastLogin = reader.IsDBNull(reader.GetOrdinal("LastLogin"))
                                    ? DateTime.MinValue
                                    : reader.GetDateTime(reader.GetOrdinal("LastLogin"))
                    });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading employees: " + ex.Message;
            }

            return View("~/Views/Home/Employees.cshtml", employees);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(int userId, string firstName, string lastName, bool status, string role)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand("UpdateEmployee", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@IsActive", status);
                command.Parameters.AddWithValue("@Role", role);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update employee: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int userId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand("EraseUserData", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@UserID", userId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Purchase()
        {
            var categories = new List<Category>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("ViewCategories", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    categories.Add(new Category
                    {
                        CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                    });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading categories: " + ex.Message;
            }

            return View("~/Views/Home/Purchase.cshtml", categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string categoryName)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand("AddCategory", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@CategoryName", categoryName);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return RedirectToAction("Purchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error adding category: " + ex.Message;
                return RedirectToAction("Purchase");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                return Json(new { success = false, message = "Category name must not be empty." });
            }

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("UpdateCategoryName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                command.Parameters.AddWithValue("@CategoryName", category.CategoryName);

                await command.ExecuteNonQueryAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory([FromBody] DeleteCategoryRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand("DeleteCategory", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@CategoryID", request.CategoryId);

                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "Category not found or already deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting category: " + ex.Message });
            }
        }

        public class DeleteCategoryRequest
        {
            public int CategoryId { get; set; }
        }

        public IActionResult Stock()
        {
            List<StockViewModel> stockItems = new List<StockViewModel>();
            List<Category> categories = new List<Category>();
            string lastUpdatedBy = "N/A";

            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open();

                // Get stock items
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetStockView";
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var stockItem = new StockViewModel
                            {
                                IngredientID = reader.GetInt32(0),
                                IngredientName = reader.GetString(1),
                                CategoryName = reader.GetString(2),
                                UnitOfMeasure = reader.GetString(3),
                                InStock = reader.GetDecimal(4),
                                DateAdded = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                LastReplenished = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                LastUpdatedBy = reader.IsDBNull(7) ? null : reader.GetString(7) // 🛠️ ADD THIS
                            };

                            stockItems.Add(stockItem);
                        }
                    }
                }

                // Get categories
                using (var categoryCommand = connection.CreateCommand())
                {
                    categoryCommand.CommandText = "ViewCategories";
                    categoryCommand.CommandType = CommandType.StoredProcedure;

                    using (var reader = categoryCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                            });
                        }
                    }
                }
            }

            // Determine latest updater
            lastUpdatedBy = stockItems
                .OrderByDescending(x => x.LastReplenished ?? x.DateAdded)
                .Select(x => x.LastUpdatedBy)
                .FirstOrDefault() ?? "N/A";

            ViewBag.LastUpdatedBy = lastUpdatedBy;
            ViewBag.Categories = categories;

            return View("~/Views/Home/Stock.cshtml", stockItems);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStock(string ingredientName, decimal inStock, string unit, int categoryId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            Console.WriteLine("===== DEBUG: Entered AddStock method =====");
            Console.WriteLine($"ingredientName: {ingredientName}");
            Console.WriteLine($"inStock: {inStock}");
            Console.WriteLine($"unit: {unit}");
            Console.WriteLine($"categoryId: {categoryId}");

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand("AddStockItem", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Name", ingredientName);
                command.Parameters.AddWithValue("@Quantity", inStock);
                command.Parameters.AddWithValue("@UnitOfMeasure", unit);
                command.Parameters.AddWithValue("@CategoryID", categoryId);

                Console.WriteLine("DEBUG: Opening SQL connection...");
                await connection.OpenAsync();
                Console.WriteLine("DEBUG: Connection opened.");

                Console.WriteLine("DEBUG: Executing stored procedure 'AddStockItem'...");
                var rowsAffected = await command.ExecuteNonQueryAsync();
                Console.WriteLine($"DEBUG: ExecuteNonQueryAsync completed. Rows affected: {rowsAffected}");

                return RedirectToAction("Stock");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("===== SQL EXCEPTION CAUGHT =====");
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                ModelState.AddModelError(string.Empty, $"Error adding ingredient: {ex.Message}");

                var stockItems = new List<StockViewModel>();

                await using var dbConnection = _context.Database.GetDbConnection();
                await dbConnection.OpenAsync();

                await using var dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = "GetStockView";
                dbCommand.CommandType = CommandType.StoredProcedure;

                await using var reader = await dbCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    stockItems.Add(new StockViewModel
                    {
                        IngredientID = reader.GetInt32(0),
                        IngredientName = reader.GetString(1),
                        CategoryName = reader.GetString(2),
                        UnitOfMeasure = reader.GetString(3),
                        InStock = reader.GetDecimal(4),
                        DateAdded = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                        LastReplenished = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                    });
                }

                Console.WriteLine("===== DEBUG: Reloaded stock view after error =====");

                return View("~/Views/Home/Stock.cshtml", stockItems);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplenishStock(int ingredientId, decimal quantity, decimal cost, string operation)
        {
            try
            {
                if (operation == "subtract")
                {
                    quantity *= -1;
                }

                int userId = GetCurrentUserId();
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand("ReplenishIngredient", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IngredientID", ingredientId);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Cost", cost);
                command.Parameters.AddWithValue("@UserID", userId);
                Console.WriteLine($"ReplenishStock called with: IngredientID={ingredientId}, Quantity={quantity}, Cost={cost}, Operation={operation}, UserID={userId}");

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return RedirectToAction("Stock");
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        private int GetCurrentUserId()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("User ID is missing from session. User might not be logged in.");
            }

            return userId.Value;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIngredient(int ingredientId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand("DeleteIngredient", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IngredientID", ingredientId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                // Redirect back to the Stock page after successful delete
                return RedirectToAction("Stock");
            }
            catch (Exception ex)
            {
                // You can either redirect with an error message or show a status page
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public IActionResult Menu()
        {
            return View("~/Views/Home/Menu.cshtml");
        }

        public IActionResult OrderList()
        {
            return View("~/Views/Home/OrderList.cshtml");
        }
    }
}