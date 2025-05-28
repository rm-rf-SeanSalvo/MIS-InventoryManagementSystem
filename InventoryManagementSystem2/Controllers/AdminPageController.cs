using InventoryManagementSystem2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryManagementSystem2.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
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
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var stockList = new List<StockViewModel>();

            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();

                using var cmd = new SqlCommand("GetStockView", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    stockList.Add(new StockViewModel
                    {
                        IngredientID = reader.GetInt32(reader.GetOrdinal("IngredientID")),
                        IngredientName = reader.GetString(reader.GetOrdinal("IngredientName")),
                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                        UnitOfMeasure = reader.GetString(reader.GetOrdinal("UnitOfMeasure")),
                        InStock = reader.GetDecimal(reader.GetOrdinal("InStock")),
                        LastReplenished = reader.GetDateTime(reader.GetOrdinal("LastReplenished"))
                    });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading stock: " + ex.Message;
            }

            return View("~/Views/Home/Stock.cshtml", stockList);
        }

        public async Task<IActionResult> AddItem(string itemName, int quantity, string category, DateTime dateAdded)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand("InsertStockItem", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IngredientName", itemName);
                command.Parameters.AddWithValue("@InStock", quantity);
                command.Parameters.AddWithValue("@CategoryName", category);
                command.Parameters.AddWithValue("@ReplenishmentDate", dateAdded);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return RedirectToAction("Stock");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error adding item: " + ex.Message;
                return RedirectToAction("Stock");
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