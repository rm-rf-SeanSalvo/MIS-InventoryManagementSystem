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

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ViewEmployees", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
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
                }
            }

            return View("~/Views/Home/Employees.cshtml", employees);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(int userId, string firstName, string lastName, bool status, string role)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("UpdateEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@IsActive", status);
                    command.Parameters.AddWithValue("@Role", role);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

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
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("EraseUserData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", userId);

                        await command.ExecuteNonQueryAsync();
                    }
                }

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
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("ViewCategories", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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

            return View("~/Views/Home/Purchase.cshtml", categories);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(int categoryId, string categoryName)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("UpdateCategoryName", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CategoryID", categoryId);
                    command.Parameters.AddWithValue("@CategoryName", categoryName);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred while updating the category." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("DeleteCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CategoryID", categoryId);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred while deleting the category." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string categoryName)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("AddCategory", connection)) // Matches your procedure name
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CategoryName", categoryName);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return RedirectToAction("Purchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error adding category: " + ex.Message;
                return RedirectToAction("Purchase");
            }
        }

        public IActionResult Stock()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return View("Error");
            }

            List<StockViewModel> stockList = new List<StockViewModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("GetStockView", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
                }
            }

            return View("~/Views/Home/Stock.cshtml", stockList); 
        }

        public async Task<IActionResult> AddItem(string itemName, int quantity, string category, DateTime dateAdded)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("InsertStockItem", connection)) // Make sure this matches your stored procedure
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IngredientName", itemName);
                    command.Parameters.AddWithValue("@InStock", quantity);
                    command.Parameters.AddWithValue("@CategoryName", category); // or @CategoryID if you're passing ID
                    command.Parameters.AddWithValue("@ReplenishmentDate", dateAdded);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

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
