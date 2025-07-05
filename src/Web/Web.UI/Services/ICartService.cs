using Web.UI.Models;

namespace Web.UI.Services;

public interface ICartService
{
    Task<CartViewModel> GetCartAsync();
    Task AddToCartAsync(int productId, int quantity = 1);
    Task RemoveFromCartAsync(int productId);
    Task UpdateQuantityAsync(int productId, int quantity);
    Task ClearCartAsync();
    Task<int> GetCartItemCountAsync();
}

public class CartService : ICartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CartService> _logger;
    private const string CartSessionKey = "ShoppingCart";

    public CartService(IHttpContextAccessor httpContextAccessor, ILogger<CartService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private ISession Session => _httpContextAccessor.HttpContext?.Session ?? throw new InvalidOperationException("Session not available");

    public async Task<CartViewModel> GetCartAsync()
    {
        try
        {
            var cartJson = Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new CartViewModel();
            }

            var cart = System.Text.Json.JsonSerializer.Deserialize<CartViewModel>(cartJson);
            return cart ?? new CartViewModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart from session");
            return new CartViewModel();
        }
    }

    public async Task AddToCartAsync(int productId, int quantity = 1)
    {
        try
        {
            var cart = await GetCartAsync();
            
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.TotalPrice = existingItem.Price * existingItem.Quantity;
            }
            else
            {
                // In a real application, you would fetch product details from the API
                // For now, we'll use placeholder data
                cart.Items.Add(new CartItemViewModel
                {
                    ProductId = productId,
                    ProductName = $"Product {productId}",
                    ProductImage = "/images/placeholder.jpg",
                    Price = 100000, // Placeholder price
                    Quantity = quantity,
                    TotalPrice = 100000 * quantity
                });
            }

            await UpdateCartTotalsAsync(cart);
            await SaveCartAsync(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart");
        }
    }

    public async Task RemoveFromCartAsync(int productId)
    {
        try
        {
            var cart = await GetCartAsync();
            var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            
            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove);
                await UpdateCartTotalsAsync(cart);
                await SaveCartAsync(cart);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from cart");
        }
    }

    public async Task UpdateQuantityAsync(int productId, int quantity)
    {
        try
        {
            var cart = await GetCartAsync();
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            
            if (existingItem != null)
            {
                if (quantity <= 0)
                {
                    await RemoveFromCartAsync(productId);
                }
                else
                {
                    existingItem.Quantity = quantity;
                    existingItem.TotalPrice = existingItem.Price * quantity;
                    await UpdateCartTotalsAsync(cart);
                    await SaveCartAsync(cart);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart quantity");
        }
    }

    public async Task ClearCartAsync()
    {
        try
        {
            Session.Remove(CartSessionKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
        }
    }

    public async Task<int> GetCartItemCountAsync()
    {
        try
        {
            var cart = await GetCartAsync();
            return cart.Items.Sum(i => i.Quantity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart item count");
            return 0;
        }
    }

    private async Task UpdateCartTotalsAsync(CartViewModel cart)
    {
        cart.TotalItems = cart.Items.Sum(i => i.Quantity);
        cart.TotalPrice = cart.Items.Sum(i => i.TotalPrice);
    }

    private async Task SaveCartAsync(CartViewModel cart)
    {
        var cartJson = System.Text.Json.JsonSerializer.Serialize(cart);
        Session.SetString(CartSessionKey, cartJson);
    }
}