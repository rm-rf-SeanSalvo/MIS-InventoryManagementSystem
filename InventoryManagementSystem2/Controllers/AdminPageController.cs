using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem2.Controllers
{
    // AdminController.cs
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            // Logic for Dashboard page
            return View();
        }

        public IActionResult Employees()
        {
            // Logic for Employees page
            return View();
        }

        public IActionResult Purchase()
        {
            // Logic for Purchase page
            return View();
        }

        public IActionResult Stock()
        {
            // Logic for Stock page
            return View();
        }

        public IActionResult Menu()
        {
            // Logic for Menu page
            return View();
        }

        public IActionResult OrderList()
        {
            // Logic for Order List page
            return View();
        }
    }

}
