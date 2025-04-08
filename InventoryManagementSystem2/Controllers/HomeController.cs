using InventoryManagementSystem2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // This action is for your login page
    public IActionResult LoginPage()
    {
        return View();
    }

    public IActionResult Index()
    {
        return View(); // Admin's Home page
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
