using Web.UI.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Web.UI.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApiService _apiService;
        private readonly ICartService _cartService;
        private const string WishlistSessionKey = "Wishlist";

        public WishlistService(IHttpContextAccessor httpContextAccessor, IApiService apiService, ICartService cartService)
        {
            _httpContextAccessor = httpContextAccessor;
            _apiService = apiService;
            _cartService = cartService;
        }

        public async Task<List<WishlistItem>> GetWishlistItemsAsync()
        {
            var wishlistItems = new List<WishlistItem>();
            var wishlistJson = _httpContextAccessor.HttpContext.Session.GetString(WishlistSessionKey);
            
            if (!string.IsNullOrEmpty(wishlistJson))
            {
                var productIds = JsonSerializer.Deserialize<List<int>>(wishlistJson);
                
                foreach (var productId in productIds)
                {
                    try
                    {
                        var product = await _apiService.GetAsync<ProductViewModel>($"/api/product/{productId}");
                        if (product != null)
                        {
                            wishlistItems.Add(new WishlistItem
                            {
                                ProductId = product.Id,
                                ProductName = product.Name,
                                Price = product.Price,
                                ImageUrl = product.ImageUrl,
                                IsInStock = product.StockQuantity > 0,
                                AddedDate = DateTime.Now // This would ideally be stored in a real database
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error and continue with other products
                        Console.WriteLine($"Error loading product {productId}: {ex.Message}");
                    }
                }
            }

            return wishlistItems;
        }

        public async Task AddToWishlistAsync(int productId)
        {
            var wishlistJson = _httpContextAccessor.HttpContext.Session.GetString(WishlistSessionKey);
            var productIds = new List<int>();

            if (!string.IsNullOrEmpty(wishlistJson))
            {
                productIds = JsonSerializer.Deserialize<List<int>>(wishlistJson);
            }

            if (!productIds.Contains(productId))
            {
                productIds.Add(productId);
                var updatedWishlistJson = JsonSerializer.Serialize(productIds);
                _httpContextAccessor.HttpContext.Session.SetString(WishlistSessionKey, updatedWishlistJson);
            }
        }

        public async Task RemoveFromWishlistAsync(int productId)
        {
            var wishlistJson = _httpContextAccessor.HttpContext.Session.GetString(WishlistSessionKey);
            
            if (!string.IsNullOrEmpty(wishlistJson))
            {
                var productIds = JsonSerializer.Deserialize<List<int>>(wishlistJson);
                productIds.Remove(productId);
                
                var updatedWishlistJson = JsonSerializer.Serialize(productIds);
                _httpContextAccessor.HttpContext.Session.SetString(WishlistSessionKey, updatedWishlistJson);
            }
        }

        public async Task<int> GetWishlistCountAsync()
        {
            var wishlistJson = _httpContextAccessor.HttpContext.Session.GetString(WishlistSessionKey);
            
            if (!string.IsNullOrEmpty(wishlistJson))
            {
                var productIds = JsonSerializer.Deserialize<List<int>>(wishlistJson);
                return productIds.Count;
            }

            return 0;
        }

        public async Task MoveToCartAsync(int productId)
        {
            // Add to cart
            await _cartService.AddToCartAsync(productId, 1);
            
            // Remove from wishlist
            await RemoveFromWishlistAsync(productId);
        }

        public async Task<string> GenerateShareUrlAsync()
        {
            var wishlistItems = await GetWishlistItemsAsync();
            var productIds = wishlistItems.Select(x => x.ProductId).ToList();
            
            // Generate a unique share key (in a real application, this would be stored in database)
            var shareKey = Guid.NewGuid().ToString("N")[..8];
            
            // For now, we'll create a simple URL with product IDs
            var baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host;
            return $"{baseUrl}/Wishlist/Shared?products={string.Join(",", productIds)}";
        }

        public async Task<bool> IsInWishlistAsync(int productId)
        {
            var wishlistJson = _httpContextAccessor.HttpContext.Session.GetString(WishlistSessionKey);
            
            if (!string.IsNullOrEmpty(wishlistJson))
            {
                var productIds = JsonSerializer.Deserialize<List<int>>(wishlistJson);
                return productIds.Contains(productId);
            }

            return false;
        }

        public async Task ClearWishlistAsync()
        {
            _httpContextAccessor.HttpContext.Session.Remove(WishlistSessionKey);
        }
    }
}