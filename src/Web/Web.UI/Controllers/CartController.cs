using Microsoft.AspNetCore.Mvc;
using Web.UI.Models;
using Web.UI.Services;

namespace Web.UI.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IApiService _apiService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, IApiService apiService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var cart = await _cartService.GetCartAsync();
            ViewData["Title"] = "سبد خرید / Shopping Cart";
            
            return View(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cart");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        try
        {
            await _cartService.AddToCartAsync(productId, quantity);
            TempData["Success"] = "محصول به سبد خرید اضافه شد / Product added to cart";
            
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product to cart");
            TempData["Error"] = "خطا در افزودن محصول به سبد خرید / Error adding product to cart";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        try
        {
            await _cartService.RemoveFromCartAsync(productId);
            TempData["Success"] = "محصول از سبد خرید حذف شد / Product removed from cart";
            
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing product from cart");
            TempData["Error"] = "خطا در حذف محصول از سبد خرید / Error removing product from cart";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
    {
        try
        {
            await _cartService.UpdateQuantityAsync(productId, quantity);
            TempData["Success"] = "تعداد محصول به‌روزرسانی شد / Product quantity updated";
            
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product quantity");
            TempData["Error"] = "خطا در به‌روزرسانی تعداد محصول / Error updating product quantity";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            await _cartService.ClearCartAsync();
            TempData["Success"] = "سبد خرید خالی شد / Cart cleared";
            
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            TempData["Error"] = "خطا در خالی کردن سبد خرید / Error clearing cart";
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> Checkout()
    {
        try
        {
            var cart = await _cartService.GetCartAsync();
            
            if (cart.Items.Count == 0)
            {
                TempData["Error"] = "سبد خرید خالی است / Cart is empty";
                return RedirectToAction("Index");
            }
            
            var checkoutModel = new CheckoutViewModel
            {
                Items = cart.Items,
                TotalAmount = cart.TotalPrice
            };
            
            ViewData["Title"] = "تسویه حساب / Checkout";
            
            return View(checkoutModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading checkout");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var cart = await _cartService.GetCartAsync();
                model.Items = cart.Items;
                model.TotalAmount = cart.TotalPrice;
                return View(model);
            }

            var order = await _apiService.CreateOrderAsync(model);
            
            if (order != null)
            {
                await _cartService.ClearCartAsync();
                
                TempData["Success"] = "سفارش با موفقیت ثبت شد / Order placed successfully";
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }
            else
            {
                TempData["Error"] = "خطا در ثبت سفارش / Error placing order";
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing checkout");
            TempData["Error"] = "خطا در پردازش سفارش / Error processing order";
            return View(model);
        }
    }

    public async Task<IActionResult> OrderConfirmation(int orderId)
    {
        try
        {
            var order = await _apiService.GetOrderByIdAsync(orderId);
            
            if (order == null)
            {
                return NotFound();
            }
            
            ViewData["Title"] = "تأیید سفارش / Order Confirmation";
            
            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading order confirmation");
            return View("Error");
        }
    }

    // AJAX endpoint for cart count
    [HttpGet]
    public async Task<IActionResult> GetCartCount()
    {
        try
        {
            var count = await _cartService.GetCartItemCountAsync();
            return Json(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart count");
            return Json(new { count = 0 });
        }
    }
}