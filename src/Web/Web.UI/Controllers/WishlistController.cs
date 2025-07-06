using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web.UI.Models;
using Web.UI.Services;

namespace Web.UI.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IWishlistService _wishlistService;

        public WishlistController(IApiService apiService, IWishlistService wishlistService)
        {
            _apiService = apiService;
            _wishlistService = wishlistService;
        }

        public async Task<IActionResult> Index()
        {
            var wishlistItems = await _wishlistService.GetWishlistItemsAsync();
            var model = new WishlistViewModel
            {
                Items = wishlistItems,
                TotalItems = wishlistItems.Count
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            try
            {
                await _wishlistService.AddToWishlistAsync(productId);
                return Json(new { success = true, message = "محصول به لیست علاقه‌مندی اضافه شد" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            try
            {
                await _wishlistService.RemoveFromWishlistAsync(productId);
                return Json(new { success = true, message = "محصول از لیست علاقه‌مندی حذف شد" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlistCount()
        {
            var count = await _wishlistService.GetWishlistCountAsync();
            return Json(new { count = count });
        }

        [HttpPost]
        public async Task<IActionResult> MoveToCart(int productId)
        {
            try
            {
                await _wishlistService.MoveToCartAsync(productId);
                return Json(new { success = true, message = "محصول به سبد خرید منتقل شد" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ShareWishlist()
        {
            try
            {
                var shareUrl = await _wishlistService.GenerateShareUrlAsync();
                return Json(new { success = true, shareUrl = shareUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}