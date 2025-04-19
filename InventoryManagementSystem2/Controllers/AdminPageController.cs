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
        public async Task<IActionResult> UpdateEmployee(int userId, string firstName, string lastName, int status, string role)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("UpdateEmployee", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@IsActive", status);
                        command.Parameters.AddWithValue("@Role", role);

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

        public IActionResult Purchase()
        {
            return View("~/Views/Home/Purchase.cshtml");
        }

        public IActionResult Stock()
        {
            return View("~/Views/Home/Stock.cshtml");
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
