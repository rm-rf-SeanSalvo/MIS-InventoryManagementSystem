using Microsoft.Data.SqlClient;
using System;

namespace InventoryManagementSystem2.Services
{
    public class DBConnectionTester
    {
        private readonly string _connectionString;

        public DBConnectionTester(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void TestConnection()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("SQL Connection successful!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SQL Connection failed: " + ex.Message);
                }
            }
        }
    }
}
