using Web.UI.Models;

namespace Web.UI.Services
{
    public interface IWishlistService
    {
        Task<List<WishlistItem>> GetWishlistItemsAsync();
        Task AddToWishlistAsync(int productId);
        Task RemoveFromWishlistAsync(int productId);
        Task<int> GetWishlistCountAsync();
        Task MoveToCartAsync(int productId);
        Task<string> GenerateShareUrlAsync();
        Task<bool> IsInWishlistAsync(int productId);
        Task ClearWishlistAsync();
    }
}