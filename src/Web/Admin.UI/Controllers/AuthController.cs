using Microsoft.AspNetCore.Mvc;
using Admin.UI.Models;
using Admin.UI.Services;

namespace Admin.UI.Controllers;

[Route("Auth")]
public class AuthController : Controller
{
    private readonly IAdminApiService _adminApiService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAdminApiService adminApiService, ILogger<AuthController> logger)
    {
        _adminApiService = adminApiService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["Title"] = "Admin Login";
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var token = await _adminApiService.LoginAsync(model);
            
            if (!string.IsNullOrEmpty(token))
            {
                HttpContext.Session.SetString("AdminAuthToken", token);
                
                TempData["Success"] = "Login successful";
                
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                
                return RedirectToAction("Index", "Dashboard");
            }
            
            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin login");
            ModelState.AddModelError("", "Login error");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            HttpContext.Session.Remove("AdminAuthToken");
            TempData["Success"] = "Logout successful";
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return RedirectToAction("Login");
        }
    }
}