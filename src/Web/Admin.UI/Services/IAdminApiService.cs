using Admin.UI.Models;

namespace Admin.UI.Services;

public interface IAdminApiService
{
    // Dashboard
    Task<DashboardViewModel> GetDashboardDataAsync();
    Task<DashboardStats> GetDashboardStatsAsync();
    
    // Product Management
    Task<ProductManagementViewModel> GetProductsAsync(int page = 1, int pageSize = 20, string? searchTerm = null, int? categoryId = null, string? sortBy = null);
    Task<ProductViewModel?> GetProductByIdAsync(int id);
    Task<bool> CreateProductAsync(CreateProductViewModel model);
    Task<bool> UpdateProductAsync(EditProductViewModel model);
    Task<bool> DeleteProductAsync(int id);
    
    // Category Management
    Task<List<CategoryViewModel>> GetCategoriesAsync();
    Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
    Task<bool> CreateCategoryAsync(CategoryViewModel model);
    Task<bool> UpdateCategoryAsync(CategoryViewModel model);
    Task<bool> DeleteCategoryAsync(int id);
    
    // Order Management
    Task<OrderManagementViewModel> GetOrdersAsync(int page = 1, int pageSize = 20, string? searchTerm = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<OrderViewModel?> GetOrderByIdAsync(int id);
    Task<bool> UpdateOrderStatusAsync(int id, string status);
    Task<bool> UpdatePaymentStatusAsync(int id, string paymentStatus);
    Task<bool> DeleteOrderAsync(int id);
    
    // Customer Management
    Task<CustomerManagementViewModel> GetCustomersAsync(int page = 1, int pageSize = 20, string? searchTerm = null, bool? isActive = null);
    Task<CustomerViewModel?> GetCustomerByIdAsync(string id);
    Task<bool> ToggleCustomerStatusAsync(string id);
    
    // Reports
    Task<SalesReportViewModel> GetSalesReportAsync(DateTime startDate, DateTime endDate);
    Task<List<TopProductViewModel>> GetTopProductsAsync(int count = 10);
    Task<List<ChartDataViewModel>> GetSalesChartDataAsync(DateTime startDate, DateTime endDate);
    
    // Settings
    Task<SettingsViewModel> GetSettingsAsync();
    Task<bool> UpdateSettingsAsync(SettingsViewModel model);
    
    // Authentication
    Task<string?> LoginAsync(LoginViewModel model);
    Task<UserProfileViewModel?> GetUserProfileAsync();
    Task LogoutAsync();
}

public class AdminApiService : IAdminApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminApiService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminApiService(HttpClient httpClient, IConfiguration configuration, ILogger<AdminApiService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private void SetAuthorizationHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("AdminAuthToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        try
        {
            SetAuthorizationHeader();
            
            // Get dashboard stats
            var stats = await GetDashboardStatsAsync();
            
            // Get recent orders (simulated)
            var recentOrders = new List<RecentOrderViewModel>
            {
                new() { Id = 1, OrderNumber = "ORD-001", CustomerName = "علی احمدی", TotalAmount = 250000, Status = "در انتظار", OrderDate = DateTime.Now.AddDays(-1) },
                new() { Id = 2, OrderNumber = "ORD-002", CustomerName = "فاطمه رضایی", TotalAmount = 180000, Status = "تأیید شده", OrderDate = DateTime.Now.AddDays(-2) },
                new() { Id = 3, OrderNumber = "ORD-003", CustomerName = "محمد کریمی", TotalAmount = 320000, Status = "ارسال شده", OrderDate = DateTime.Now.AddDays(-3) }
            };
            
            // Get top products (simulated)
            var topProducts = new List<TopProductViewModel>
            {
                new() { Id = 1, NamePersian = "گوشی Samsung Galaxy", NameEnglish = "Samsung Galaxy", SoldQuantity = 25, Revenue = 1250000 },
                new() { Id = 2, NamePersian = "لپ‌تاپ Asus", NameEnglish = "Asus Laptop", SoldQuantity = 12, Revenue = 2400000 },
                new() { Id = 3, NamePersian = "هدفون Sony", NameEnglish = "Sony Headphones", SoldQuantity = 35, Revenue = 875000 }
            };
            
            // Get sales chart data (simulated)
            var salesChart = new List<ChartDataViewModel>
            {
                new() { Label = "شنبه", Value = 150000 },
                new() { Label = "یکشنبه", Value = 220000 },
                new() { Label = "دوشنبه", Value = 180000 },
                new() { Label = "سه‌شنبه", Value = 290000 },
                new() { Label = "چهارشنبه", Value = 240000 },
                new() { Label = "پنج‌شنبه", Value = 320000 },
                new() { Label = "جمعه", Value = 280000 }
            };
            
            return new DashboardViewModel
            {
                Stats = stats,
                RecentOrders = recentOrders,
                TopProducts = topProducts,
                SalesChart = salesChart
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data");
            return new DashboardViewModel();
        }
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        try
        {
            SetAuthorizationHeader();
            
            // In a real implementation, these would be API calls
            // For now, return simulated data
            return new DashboardStats
            {
                TotalProducts = 1250,
                TotalOrders = 3420,
                TotalCustomers = 850,
                TotalRevenue = 15750000,
                PendingOrders = 25,
                LowStockProducts = 8,
                TodayRevenue = 280000,
                MonthlyRevenue = 8500000
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return new DashboardStats();
        }
    }

    public async Task<ProductManagementViewModel> GetProductsAsync(int page = 1, int pageSize = 20, string? searchTerm = null, int? categoryId = null, string? sortBy = null)
    {
        try
        {
            SetAuthorizationHeader();
            
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrEmpty(searchTerm))
                queryParams.Add($"search={Uri.EscapeDataString(searchTerm)}");
            
            if (categoryId.HasValue)
                queryParams.Add($"categoryId={categoryId.Value}");
            
            if (!string.IsNullOrEmpty(sortBy))
                queryParams.Add($"sortBy={sortBy}");

            var query = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:ProductApi"]}/api/product?{query}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = System.Text.Json.JsonSerializer.Deserialize<List<ProductViewModel>>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var categories = await GetCategoriesAsync();
                
                return new ProductManagementViewModel
                {
                    Products = products ?? new List<ProductViewModel>(),
                    Categories = categories,
                    TotalProducts = products?.Count ?? 0,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((products?.Count ?? 0) / (double)pageSize),
                    SearchTerm = searchTerm,
                    CategoryId = categoryId,
                    SortBy = sortBy
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
        }

        return new ProductManagementViewModel();
    }

    public async Task<ProductViewModel?> GetProductByIdAsync(int id)
    {
        try
        {
            SetAuthorizationHeader();
            
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:ProductApi"]}/api/product/{id}");
            
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
            _logger.LogError(ex, "Error getting product {ProductId}", id);
        }

        return null;
    }

    public async Task<bool> CreateProductAsync(CreateProductViewModel model)
    {
        try
        {
            SetAuthorizationHeader();
            
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:ProductApi"]}/api/product", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return false;
        }
    }

    public async Task<bool> UpdateProductAsync(EditProductViewModel model)
    {
        try
        {
            SetAuthorizationHeader();
            
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{_configuration["ApiSettings:ProductApi"]}/api/product/{model.Id}", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product");
            return false;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            SetAuthorizationHeader();
            
            var response = await _httpClient.DeleteAsync($"{_configuration["ApiSettings:ProductApi"]}/api/product/{id}");
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product");
            return false;
        }
    }

    public async Task<List<CategoryViewModel>> GetCategoriesAsync()
    {
        try
        {
            SetAuthorizationHeader();
            
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:ProductApi"]}/api/category");
            
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
            _logger.LogError(ex, "Error getting categories");
        }

        return new List<CategoryViewModel>();
    }

    public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
    {
        try
        {
            SetAuthorizationHeader();
            
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:ProductApi"]}/api/category/{id}");
            
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
            _logger.LogError(ex, "Error getting category {CategoryId}", id);
        }

        return null;
    }

    public async Task<bool> CreateCategoryAsync(CategoryViewModel model)
    {
        try
        {
            SetAuthorizationHeader();
            
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:ProductApi"]}/api/category", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return false;
        }
    }

    public async Task<bool> UpdateCategoryAsync(CategoryViewModel model)
    {
        try
        {
            SetAuthorizationHeader();
            
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{_configuration["ApiSettings:ProductApi"]}/api/category/{model.Id}", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            return false;
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            SetAuthorizationHeader();
            
            var response = await _httpClient.DeleteAsync($"{_configuration["ApiSettings:ProductApi"]}/api/category/{id}");
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            return false;
        }
    }

    public async Task<OrderManagementViewModel> GetOrdersAsync(int page = 1, int pageSize = 20, string? searchTerm = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            SetAuthorizationHeader();
            
            // Simulated data for now
            var orders = new List<OrderViewModel>
            {
                new() { Id = 1, OrderNumber = "ORD-001", CustomerName = "علی احمدی", CustomerEmail = "ali@example.com", TotalAmount = 250000, Status = "در انتظار", PaymentStatus = "پرداخت نشده", OrderDate = DateTime.Now.AddDays(-1) },
                new() { Id = 2, OrderNumber = "ORD-002", CustomerName = "فاطمه رضایی", CustomerEmail = "fateme@example.com", TotalAmount = 180000, Status = "تأیید شده", PaymentStatus = "پرداخت شده", OrderDate = DateTime.Now.AddDays(-2) },
                new() { Id = 3, OrderNumber = "ORD-003", CustomerName = "محمد کریمی", CustomerEmail = "mohammad@example.com", TotalAmount = 320000, Status = "ارسال شده", PaymentStatus = "پرداخت شده", OrderDate = DateTime.Now.AddDays(-3) }
            };
            
            return new OrderManagementViewModel
            {
                Orders = orders,
                TotalOrders = orders.Count,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = 1,
                SearchTerm = searchTerm,
                Status = status,
                StartDate = startDate,
                EndDate = endDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders");
            return new OrderManagementViewModel();
        }
    }

    public async Task<OrderViewModel?> GetOrderByIdAsync(int id)
    {
        try
        {
            SetAuthorizationHeader();
            
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:OrderApi"]}/api/order/{id}");
            
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
            _logger.LogError(ex, "Error getting order {OrderId}", id);
        }

        return null;
    }

    public async Task<bool> UpdateOrderStatusAsync(int id, string status)
    {
        try
        {
            SetAuthorizationHeader();
            
            var payload = new { status };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{_configuration["ApiSettings:OrderApi"]}/api/order/{id}/status", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status");
            return false;
        }
    }

    public async Task<bool> UpdatePaymentStatusAsync(int id, string paymentStatus)
    {
        try
        {
            SetAuthorizationHeader();
            
            var payload = new { paymentStatus };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{_configuration["ApiSettings:OrderApi"]}/api/order/{id}/payment-status", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status");
            return false;
        }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            SetAuthorizationHeader();
            
            var response = await _httpClient.DeleteAsync($"{_configuration["ApiSettings:OrderApi"]}/api/order/{id}");
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order");
            return false;
        }
    }

    public async Task<CustomerManagementViewModel> GetCustomersAsync(int page = 1, int pageSize = 20, string? searchTerm = null, bool? isActive = null)
    {
        try
        {
            SetAuthorizationHeader();
            
            // Simulated data for now
            var customers = new List<CustomerViewModel>
            {
                new() { Id = "1", Email = "ali@example.com", FirstName = "علی", LastName = "احمدی", Phone = "09123456789", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-6), TotalOrders = 5, TotalSpent = 1250000 },
                new() { Id = "2", Email = "fateme@example.com", FirstName = "فاطمه", LastName = "رضایی", Phone = "09134567890", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-4), TotalOrders = 3, TotalSpent = 780000 },
                new() { Id = "3", Email = "mohammad@example.com", FirstName = "محمد", LastName = "کریمی", Phone = "09145678901", IsActive = false, CreatedAt = DateTime.Now.AddMonths(-8), TotalOrders = 2, TotalSpent = 450000 }
            };
            
            return new CustomerManagementViewModel
            {
                Customers = customers,
                TotalCustomers = customers.Count,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = 1,
                SearchTerm = searchTerm,
                IsActive = isActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customers");
            return new CustomerManagementViewModel();
        }
    }

    public async Task<CustomerViewModel?> GetCustomerByIdAsync(string id)
    {
        try
        {
            SetAuthorizationHeader();
            
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:IdentityApi"]}/api/customer/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<CustomerViewModel>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer {CustomerId}", id);
        }

        return null;
    }

    public async Task<bool> ToggleCustomerStatusAsync(string id)
    {
        try
        {
            SetAuthorizationHeader();
            
            var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:IdentityApi"]}/api/customer/{id}/toggle-status", null);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling customer status");
            return false;
        }
    }

    public async Task<SalesReportViewModel> GetSalesReportAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            SetAuthorizationHeader();
            
            // Simulated data for now
            return new SalesReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalSales = 5280000,
                TotalOrders = 127,
                AverageOrderValue = 41574
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sales report");
            return new SalesReportViewModel();
        }
    }

    public async Task<List<TopProductViewModel>> GetTopProductsAsync(int count = 10)
    {
        try
        {
            SetAuthorizationHeader();
            
            // Simulated data for now
            return new List<TopProductViewModel>
            {
                new() { Id = 1, NamePersian = "گوشی Samsung Galaxy", NameEnglish = "Samsung Galaxy", SoldQuantity = 25, Revenue = 1250000 },
                new() { Id = 2, NamePersian = "لپ‌تاپ Asus", NameEnglish = "Asus Laptop", SoldQuantity = 12, Revenue = 2400000 },
                new() { Id = 3, NamePersian = "هدفون Sony", NameEnglish = "Sony Headphones", SoldQuantity = 35, Revenue = 875000 }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top products");
            return new List<TopProductViewModel>();
        }
    }

    public async Task<List<ChartDataViewModel>> GetSalesChartDataAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            SetAuthorizationHeader();
            
            // Simulated data for now
            return new List<ChartDataViewModel>
            {
                new() { Label = "شنبه", Value = 150000 },
                new() { Label = "یکشنبه", Value = 220000 },
                new() { Label = "دوشنبه", Value = 180000 },
                new() { Label = "سه‌شنبه", Value = 290000 },
                new() { Label = "چهارشنبه", Value = 240000 },
                new() { Label = "پنج‌شنبه", Value = 320000 },
                new() { Label = "جمعه", Value = 280000 }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sales chart data");
            return new List<ChartDataViewModel>();
        }
    }

    public async Task<SettingsViewModel> GetSettingsAsync()
    {
        try
        {
            SetAuthorizationHeader();
            
            // Simulated data for now
            return new SettingsViewModel
            {
                General = new GeneralSettingsViewModel
                {
                    SiteName = "فروشگاه آنلاین",
                    SiteDescription = "بهترین فروشگاه آنلاین ایران",
                    Currency = "IRR",
                    Language = "fa-IR",
                    Timezone = "Asia/Tehran",
                    MaintenanceMode = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting settings");
            return new SettingsViewModel();
        }
    }

    public async Task<bool> UpdateSettingsAsync(SettingsViewModel model)
    {
        try
        {
            SetAuthorizationHeader();
            
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            // This would be an API call to save settings
            // For now, just return true
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating settings");
            return false;
        }
    }

    public async Task<string?> LoginAsync(LoginViewModel model)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:IdentityApi"]}/api/auth/admin-login", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                return tokenResponse?["token"]?.ToString();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin login");
        }

        return null;
    }

    public async Task<UserProfileViewModel?> GetUserProfileAsync()
    {
        try
        {
            SetAuthorizationHeader();
            
            var response = await _httpClient.GetAsync($"{_configuration["ApiSettings:IdentityApi"]}/api/auth/admin-profile");
            
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
            _logger.LogError(ex, "Error getting admin profile");
        }

        return null;
    }

    public async Task LogoutAsync()
    {
        try
        {
            SetAuthorizationHeader();
            
            await _httpClient.PostAsync($"{_configuration["ApiSettings:IdentityApi"]}/api/auth/admin-logout", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin logout");
        }
    }
}