using Web.UI.Models;

namespace Web.UI.Services;

public interface IApiService
{
    // Product API
    Task<ProductListViewModel> GetProductsAsync(int page = 1, int pageSize = 12, string? searchTerm = null, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, string? sortBy = null);
    Task<ProductViewModel?> GetProductByIdAsync(int id);
    Task<List<ProductViewModel>> GetFeaturedProductsAsync();
    Task<List<ProductViewModel>> GetOnSaleProductsAsync();
    Task<List<CategoryViewModel>> GetCategoriesAsync();
    Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
    
    // Order API
    Task<OrderViewModel?> CreateOrderAsync(CheckoutViewModel checkout);
    Task<List<OrderViewModel>> GetUserOrdersAsync(string userId);
    Task<OrderViewModel?> GetOrderByIdAsync(int id);
    
    // Identity API
    Task<string?> LoginAsync(LoginViewModel model);
    Task<bool> RegisterAsync(RegisterViewModel model);
    Task<UserProfileViewModel?> GetUserProfileAsync(string token);
    Task<bool> UpdateUserProfileAsync(UserProfileViewModel model, string token);
    Task LogoutAsync(string token);
    
    // Payment API
    Task<string?> InitiatePaymentAsync(int orderId, string paymentMethod);
    Task<bool> VerifyPaymentAsync(string paymentId, string transactionId);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ProductListViewModel> GetProductsAsync(int page = 1, int pageSize = 12, string? searchTerm = null, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, string? sortBy = null)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrEmpty(searchTerm))
                queryParams.Add($"search={Uri.EscapeDataString(searchTerm)}");
            
            if (categoryId.HasValue)
                queryParams.Add($"categoryId={categoryId.Value}");
            
            if (minPrice.HasValue)
                queryParams.Add($"minPrice={minPrice.Value}");
            
            if (maxPrice.HasValue)
                queryParams.Add($"maxPrice={maxPrice.Value}");
            
            if (!string.IsNullOrEmpty(sortBy))
                queryParams.Add($"sortBy={sortBy}");

            var query = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/product?{query}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var categories = await GetCategoriesAsync();
                
                return new ProductListViewModel
                {
                    Products = products ?? new List<ProductViewModel>(),
                    Categories = categories,
                    TotalProducts = products?.Count ?? 0,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((products?.Count ?? 0) / (double)pageSize),
                    SearchTerm = searchTerm,
                    CategoryId = categoryId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SortBy = sortBy
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
        }

        return new ProductListViewModel();
    }

    public async Task<ProductViewModel?> GetProductByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/product/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<ProductViewModel>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product {ProductId}", id);
        }

        return null;
    }

    public async Task<List<ProductViewModel>> GetFeaturedProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/product/featured");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ProductViewModel>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching featured products");
        }

        return new List<ProductViewModel>();
    }

    public async Task<List<ProductViewModel>> GetOnSaleProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/product/on-sale");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ProductViewModel>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching on-sale products");
        }

        return new List<ProductViewModel>();
    }

    public async Task<List<CategoryViewModel>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/category");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<CategoryViewModel>>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<CategoryViewModel>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching categories");
        }

        return new List<CategoryViewModel>();
    }

    public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/category/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<CategoryViewModel>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching category {CategoryId}", id);
        }

        return null;
    }

    public async Task<OrderViewModel?> CreateOrderAsync(CheckoutViewModel checkout)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(checkout);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/order", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<OrderViewModel>(responseContent, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
        }

        return null;
    }

    public async Task<List<OrderViewModel>> GetUserOrdersAsync(string userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/order/user/{userId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<OrderViewModel>>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<OrderViewModel>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user orders");
        }

        return new List<OrderViewModel>();
    }

    public async Task<OrderViewModel?> GetOrderByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/order/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<OrderViewModel>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order {OrderId}", id);
        }

        return null;
    }

    public async Task<string?> LoginAsync(LoginViewModel model)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/auth/login", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                return tokenResponse?["token"]?.ToString();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
        }

        return null;
    }

    public async Task<bool> RegisterAsync(RegisterViewModel model)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/auth/register", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
        }

        return false;
    }

    public async Task<UserProfileViewModel?> GetUserProfileAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/auth/profile");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<UserProfileViewModel>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user profile");
        }

        return null;
    }

    public async Task<bool> UpdateUserProfileAsync(UserProfileViewModel model, string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/auth/profile", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
        }

        return false;
    }

    public async Task LogoutAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            await _httpClient.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/auth/logout", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
        }
    }

    public async Task<string?> InitiatePaymentAsync(int orderId, string paymentMethod)
    {
        try
        {
            var payload = new { orderId, paymentMethod };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/payment/initiate", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var paymentResponse = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                return paymentResponse?["paymentUrl"]?.ToString();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating payment");
        }

        return null;
    }

    public async Task<bool> VerifyPaymentAsync(string paymentId, string transactionId)
    {
        try
        {
            var payload = new { paymentId, transactionId };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/payment/verify", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment");
        }

        return false;
    }
}