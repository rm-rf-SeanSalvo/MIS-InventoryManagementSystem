using Microsoft.AspNetCore.Mvc;

public class OrderListController : Controller
{
    public IActionResult Index()
    {
        // You could pass your order list model here
        return View();
    }

    public IActionResult OrderDetails(int id)
    {
        // Normally you'd retrieve order details using the ID
        // For now, we'll just pass the id to the view as a placeholder
        ViewBag.OrderId = id;
        return View();
    }
}
