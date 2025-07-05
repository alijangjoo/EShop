using Microsoft.AspNetCore.Mvc;
using Web.UI.Models;
using Web.UI.Services;

namespace Web.UI.Controllers;

public class ProductController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IApiService apiService, ILogger<ProductController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 12, string? search = null, 
        int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, string? sortBy = null)
    {
        try
        {
            var products = await _apiService.GetProductsAsync(page, pageSize, search, categoryId, minPrice, maxPrice, sortBy);
            
            ViewData["Title"] = "محصولات / Products";
            ViewData["CurrentCategory"] = categoryId;
            ViewData["SearchTerm"] = search;
            
            return View(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products");
            return View("Error");
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var product = await _apiService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                return NotFound();
            }
            
            ViewData["Title"] = $"{product.NamePersian} - {product.NameEnglish}";
            
            return View(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading product details for ID: {ProductId}", id);
            return View("Error");
        }
    }

    public async Task<IActionResult> Category(int id, int page = 1, int pageSize = 12)
    {
        try
        {
            var category = await _apiService.GetCategoryByIdAsync(id);
            var products = await _apiService.GetProductsAsync(page, pageSize, categoryId: id);
            
            if (category == null)
            {
                return NotFound();
            }
            
            ViewData["Title"] = $"{category.NamePersian} - {category.NameEnglish}";
            ViewData["Category"] = category;
            
            return View("Index", products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading category products for ID: {CategoryId}", id);
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term, int page = 1, int pageSize = 12)
    {
        try
        {
            var products = await _apiService.GetProductsAsync(page, pageSize, term);
            
            ViewData["Title"] = $"جستجو: {term} / Search: {term}";
            ViewData["SearchTerm"] = term;
            
            return View("Index", products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term: {SearchTerm}", term);
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Featured()
    {
        try
        {
            var products = await _apiService.GetFeaturedProductsAsync();
            
            var viewModel = new ProductListViewModel
            {
                Products = products,
                Categories = await _apiService.GetCategoriesAsync()
            };
            
            ViewData["Title"] = "محصولات ویژه / Featured Products";
            
            return View("Index", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading featured products");
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> OnSale()
    {
        try
        {
            var products = await _apiService.GetOnSaleProductsAsync();
            
            var viewModel = new ProductListViewModel
            {
                Products = products,
                Categories = await _apiService.GetCategoriesAsync()
            };
            
            ViewData["Title"] = "محصولات تخفیف‌دار / On Sale Products";
            
            return View("Index", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading on-sale products");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        try
        {
            // This would typically be handled by a cart service
            // For now, we'll redirect to the cart page
            TempData["Success"] = "محصول به سبد خرید اضافه شد / Product added to cart";
            
            return RedirectToAction("Details", new { id = productId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product to cart");
            TempData["Error"] = "خطا در افزودن محصول به سبد خرید / Error adding product to cart";
            return RedirectToAction("Details", new { id = productId });
        }
    }
}