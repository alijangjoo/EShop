using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Admin.UI.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IHttpClientFactory httpClientFactory, ILogger<DashboardController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            ViewData["Title"] = "پنل مدیریت / Admin Dashboard";
            
            // Here we would call various APIs to get dashboard statistics
            // var httpClient = _httpClientFactory.CreateClient();
            // Get products count, orders count, revenue, etc.
            
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            return View("Error");
        }
    }

    public IActionResult Products()
    {
        ViewData["Title"] = "مدیریت محصولات / Product Management";
        return View();
    }

    public IActionResult Orders()
    {
        ViewData["Title"] = "مدیریت سفارشات / Order Management";
        return View();
    }

    public IActionResult Customers()
    {
        ViewData["Title"] = "مدیریت مشتریان / Customer Management";
        return View();
    }

    public IActionResult Reports()
    {
        ViewData["Title"] = "گزارشات / Reports";
        return View();
    }

    public IActionResult Settings()
    {
        ViewData["Title"] = "تنظیمات / Settings";
        return View();
    }
}