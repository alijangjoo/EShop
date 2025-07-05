using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.UI.Models;
using Web.UI.Services;

namespace Web.UI.Controllers;

public class AccountController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IApiService apiService, ILogger<AccountController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["Title"] = "ورود / Login";
        ViewData["ReturnUrl"] = returnUrl;
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var token = await _apiService.LoginAsync(model);
            
            if (!string.IsNullOrEmpty(token))
            {
                // Store token in session or cookie
                HttpContext.Session.SetString("AuthToken", token);
                
                TempData["Success"] = "با موفقیت وارد شدید / Login successful";
                
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "ایمیل یا رمز عبور اشتباه است / Invalid email or password");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            ModelState.AddModelError("", "خطا در ورود / Login error");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewData["Title"] = "ثبت نام / Register";
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _apiService.RegisterAsync(model);
            
            if (result)
            {
                TempData["Success"] = "ثبت نام با موفقیت انجام شد / Registration successful";
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "خطا در ثبت نام / Registration error");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            ModelState.AddModelError("", "خطا در ثبت نام / Registration error");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                await _apiService.LogoutAsync(token);
            }
            
            HttpContext.Session.Remove("AuthToken");
            
            TempData["Success"] = "با موفقیت خارج شدید / Logout successful";
            
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            
            var profile = await _apiService.GetUserProfileAsync(token);
            
            if (profile == null)
            {
                return RedirectToAction("Login");
            }
            
            ViewData["Title"] = "پروفایل / Profile";
            
            return View(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user profile");
            return RedirectToAction("Login");
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(UserProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            
            var result = await _apiService.UpdateUserProfileAsync(model, token);
            
            if (result)
            {
                TempData["Success"] = "پروفایل با موفقیت به‌روزرسانی شد / Profile updated successfully";
            }
            else
            {
                TempData["Error"] = "خطا در به‌روزرسانی پروفایل / Error updating profile";
            }
            
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            TempData["Error"] = "خطا در به‌روزرسانی پروفایل / Error updating profile";
            return View(model);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Orders()
    {
        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            
            var profile = await _apiService.GetUserProfileAsync(token);
            if (profile == null)
            {
                return RedirectToAction("Login");
            }
            
            var orders = await _apiService.GetUserOrdersAsync(profile.Id);
            
            ViewData["Title"] = "سفارشات / Orders";
            
            return View(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user orders");
            return View("Error");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> OrderDetails(int id)
    {
        try
        {
            var order = await _apiService.GetOrderByIdAsync(id);
            
            if (order == null)
            {
                return NotFound();
            }
            
            ViewData["Title"] = $"جزئیات سفارش #{order.OrderNumber} / Order Details #{order.OrderNumber}";
            
            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading order details");
            return View("Error");
        }
    }
}