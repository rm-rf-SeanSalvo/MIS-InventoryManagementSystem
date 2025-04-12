using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem2.Controllers
{

    public class AdminController : Controller
    {
        public IActionResult Index()
        {

            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult Employees()
        {

            return View("~/Views/Home/Employees.cshtml");
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
