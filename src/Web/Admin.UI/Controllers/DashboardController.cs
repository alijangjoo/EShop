using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Admin.UI.Services;
using Admin.UI.Models;

namespace Admin.UI.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IAdminApiService _adminApiService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IAdminApiService adminApiService, ILogger<DashboardController> logger)
    {
        _adminApiService = adminApiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            ViewData["Title"] = "داشبورد / Dashboard";
            
            var dashboardData = await _adminApiService.GetDashboardDataAsync();
            
            return View(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            TempData["Error"] = "خطا در بارگذاری داشبورد / Error loading dashboard";
            return View(new DashboardViewModel());
        }
    }

    public async Task<IActionResult> Products(int page = 1, int pageSize = 20, string? search = null, int? categoryId = null, string? sortBy = null)
    {
        try
        {
            ViewData["Title"] = "مدیریت محصولات / Product Management";
            
            var products = await _adminApiService.GetProductsAsync(page, pageSize, search, categoryId, sortBy);
            
            return View(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products");
            TempData["Error"] = "خطا در بارگذاری محصولات / Error loading products";
            return View(new ProductManagementViewModel());
        }
    }

    [HttpGet]
    public async Task<IActionResult> CreateProduct()
    {
        try
        {
            ViewData["Title"] = "ایجاد محصول جدید / Create New Product";
            
            var categories = await _adminApiService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            
            return View(new CreateProductViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading create product page");
            return RedirectToAction(nameof(Products));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(CreateProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _adminApiService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            return View(model);
        }

        try
        {
            var result = await _adminApiService.CreateProductAsync(model);
            
            if (result)
            {
                TempData["Success"] = "محصول با موفقیت ایجاد شد / Product created successfully";
                return RedirectToAction(nameof(Products));
            }
            else
            {
                TempData["Error"] = "خطا در ایجاد محصول / Error creating product";
                var categories = await _adminApiService.GetCategoriesAsync();
                ViewBag.Categories = categories;
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            TempData["Error"] = "خطا در ایجاد محصول / Error creating product";
            var categories = await _adminApiService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        try
        {
            var product = await _adminApiService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                TempData["Error"] = "محصول یافت نشد / Product not found";
                return RedirectToAction(nameof(Products));
            }
            
            ViewData["Title"] = $"ویرایش محصول / Edit Product - {product.NamePersian}";
            
            var categories = await _adminApiService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            
            var editModel = new EditProductViewModel
            {
                Id = product.Id,
                NamePersian = product.NamePersian,
                NameEnglish = product.NameEnglish,
                DescriptionPersian = product.DescriptionPersian,
                DescriptionEnglish = product.DescriptionEnglish,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                IsOnSale = product.IsOnSale,
                Stock = product.Stock,
                IsAvailable = product.IsAvailable,
                IsFeatured = product.IsFeatured,
                CategoryId = product.CategoryId,
                Brand = product.Brand,
                SKU = product.SKU,
                ImageUrls = product.Images,
                Attributes = product.Attributes,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
            
            return View(editModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading product for edit");
            TempData["Error"] = "خطا در بارگذاری محصول / Error loading product";
            return RedirectToAction(nameof(Products));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(EditProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _adminApiService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            return View(model);
        }

        try
        {
            var result = await _adminApiService.UpdateProductAsync(model);
            
            if (result)
            {
                TempData["Success"] = "محصول با موفقیت به‌روزرسانی شد / Product updated successfully";
                return RedirectToAction(nameof(Products));
            }
            else
            {
                TempData["Error"] = "خطا در به‌روزرسانی محصول / Error updating product";
                var categories = await _adminApiService.GetCategoriesAsync();
                ViewBag.Categories = categories;
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product");
            TempData["Error"] = "خطا در به‌روزرسانی محصول / Error updating product";
            var categories = await _adminApiService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var result = await _adminApiService.DeleteProductAsync(id);
            
            if (result)
            {
                TempData["Success"] = "محصول با موفقیت حذف شد / Product deleted successfully";
            }
            else
            {
                TempData["Error"] = "خطا در حذف محصول / Error deleting product";
            }
            
            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product");
            TempData["Error"] = "خطا در حذف محصول / Error deleting product";
            return RedirectToAction(nameof(Products));
        }
    }

    public async Task<IActionResult> Orders(int page = 1, int pageSize = 20, string? search = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            ViewData["Title"] = "مدیریت سفارشات / Order Management";
            
            var orders = await _adminApiService.GetOrdersAsync(page, pageSize, search, status, startDate, endDate);
            
            return View(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading orders");
            TempData["Error"] = "خطا در بارگذاری سفارشات / Error loading orders";
            return View(new OrderManagementViewModel());
        }
    }

    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id)
    {
        try
        {
            var order = await _adminApiService.GetOrderByIdAsync(id);
            
            if (order == null)
            {
                TempData["Error"] = "سفارش یافت نشد / Order not found";
                return RedirectToAction(nameof(Orders));
            }
            
            ViewData["Title"] = $"جزئیات سفارش / Order Details - {order.OrderNumber}";
            
            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading order details");
            TempData["Error"] = "خطا در بارگذاری جزئیات سفارش / Error loading order details";
            return RedirectToAction(nameof(Orders));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int id, string status)
    {
        try
        {
            var result = await _adminApiService.UpdateOrderStatusAsync(id, status);
            
            if (result)
            {
                TempData["Success"] = "وضعیت سفارش با موفقیت به‌روزرسانی شد / Order status updated successfully";
            }
            else
            {
                TempData["Error"] = "خطا در به‌روزرسانی وضعیت سفارش / Error updating order status";
            }
            
            return RedirectToAction(nameof(OrderDetails), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status");
            TempData["Error"] = "خطا در به‌روزرسانی وضعیت سفارش / Error updating order status";
            return RedirectToAction(nameof(OrderDetails), new { id });
        }
    }

    public async Task<IActionResult> Customers(int page = 1, int pageSize = 20, string? search = null, bool? isActive = null)
    {
        try
        {
            ViewData["Title"] = "مدیریت مشتریان / Customer Management";
            
            var customers = await _adminApiService.GetCustomersAsync(page, pageSize, search, isActive);
            
            return View(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customers");
            TempData["Error"] = "خطا در بارگذاری مشتریان / Error loading customers";
            return View(new CustomerManagementViewModel());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCustomerStatus(string id)
    {
        try
        {
            var result = await _adminApiService.ToggleCustomerStatusAsync(id);
            
            if (result)
            {
                TempData["Success"] = "وضعیت مشتری با موفقیت تغییر یافت / Customer status changed successfully";
            }
            else
            {
                TempData["Error"] = "خطا در تغییر وضعیت مشتری / Error changing customer status";
            }
            
            return RedirectToAction(nameof(Customers));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling customer status");
            TempData["Error"] = "خطا در تغییر وضعیت مشتری / Error changing customer status";
            return RedirectToAction(nameof(Customers));
        }
    }

    public async Task<IActionResult> Reports()
    {
        try
        {
            ViewData["Title"] = "گزارشات / Reports";
            
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;
            
            var salesReport = await _adminApiService.GetSalesReportAsync(startDate, endDate);
            
            return View(salesReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reports");
            TempData["Error"] = "خطا در بارگذاری گزارشات / Error loading reports";
            return View(new SalesReportViewModel());
        }
    }

    public async Task<IActionResult> Settings()
    {
        try
        {
            ViewData["Title"] = "تنظیمات / Settings";
            
            var settings = await _adminApiService.GetSettingsAsync();
            
            return View(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading settings");
            TempData["Error"] = "خطا در بارگذاری تنظیمات / Error loading settings";
            return View(new SettingsViewModel());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(SettingsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _adminApiService.UpdateSettingsAsync(model);
            
            if (result)
            {
                TempData["Success"] = "تنظیمات با موفقیت به‌روزرسانی شد / Settings updated successfully";
            }
            else
            {
                TempData["Error"] = "خطا در به‌روزرسانی تنظیمات / Error updating settings";
            }
            
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating settings");
            TempData["Error"] = "خطا در به‌روزرسانی تنظیمات / Error updating settings";
            return View(model);
        }
    }

    // API endpoints for AJAX calls
    [HttpGet]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var stats = await _adminApiService.GetDashboardStatsAsync();
            return Json(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return Json(new { error = "Error loading stats" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSalesChartData(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            startDate ??= DateTime.Now.AddDays(-7);
            endDate ??= DateTime.Now;
            
            var chartData = await _adminApiService.GetSalesChartDataAsync(startDate.Value, endDate.Value);
            return Json(chartData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sales chart data");
            return Json(new { error = "Error loading chart data" });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}