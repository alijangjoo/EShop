using Microsoft.AspNetCore.Mvc;

namespace Web.UI.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // This will call the Product API to get featured products
            var httpClient = _httpClientFactory.CreateClient();
            // httpClient.BaseAddress = new Uri("https://localhost:5002"); // Product API URL
            
            // For now, return a simple view
            ViewData["Title"] = "فروشگاه آنلاین / Online Store";
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page");
            return View("Error");
        }
    }

    public IActionResult About()
    {
        ViewData["Title"] = "درباره ما / About Us";
        return View();
    }

    public IActionResult Contact()
    {
        ViewData["Title"] = "تماس با ما / Contact Us";
        return View();
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "حریم خصوصی / Privacy Policy";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}