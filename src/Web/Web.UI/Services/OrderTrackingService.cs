using Web.UI.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Web.UI.Services
{
    public class OrderTrackingService : IOrderTrackingService
    {
        private readonly IApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderTrackingService(IApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<OrderTrackingViewModel> GetOrderTrackingAsync(string orderNumber, string email)
        {
            try
            {
                var response = await _apiService.GetAsync<OrderTrackingViewModel>(
                    $"/api/order/track?orderNumber={orderNumber}&email={email}");
                
                if (response != null)
                {
                    // Populate tracking updates
                    response.TrackingUpdates = await GetTrackingUpdatesAsync(response.Id);
                }

                return response;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error tracking order: {ex.Message}");
                throw;
            }
        }

        public async Task<OrderTrackingViewModel> GetOrderTrackingByIdAsync(int orderId)
        {
            try
            {
                var response = await _apiService.GetAsync<OrderTrackingViewModel>($"/api/order/{orderId}");
                
                if (response != null)
                {
                    // Populate tracking updates
                    response.TrackingUpdates = await GetTrackingUpdatesAsync(response.Id);
                }

                return response;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error getting order tracking: {ex.Message}");
                throw;
            }
        }

        public async Task<List<OrderTrackingViewModel>> GetUserOrdersAsync()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return new List<OrderTrackingViewModel>();
                }

                var response = await _apiService.GetAsync<List<OrderTrackingViewModel>>($"/api/order/user/{userId}");
                
                return response ?? new List<OrderTrackingViewModel>();
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error getting user orders: {ex.Message}");
                throw;
            }
        }

        public async Task<List<TrackingUpdateViewModel>> GetTrackingUpdatesAsync(int orderId)
        {
            try
            {
                var response = await _apiService.GetAsync<List<TrackingUpdateViewModel>>($"/api/order/{orderId}/tracking");
                
                return response ?? GenerateMockTrackingUpdates(orderId);
            }
            catch (Exception ex)
            {
                // Log error and return mock data for demonstration
                Console.WriteLine($"Error getting tracking updates: {ex.Message}");
                return GenerateMockTrackingUpdates(orderId);
            }
        }

        public async Task RequestDeliveryUpdateAsync(int orderId, string customerPhone)
        {
            try
            {
                var request = new { OrderId = orderId, CustomerPhone = customerPhone };
                await _apiService.PostAsync("/api/order/request-delivery-update", request);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error requesting delivery update: {ex.Message}");
                throw;
            }
        }

        public async Task CancelOrderAsync(int orderId, string reason)
        {
            try
            {
                var request = new { OrderId = orderId, Reason = reason };
                await _apiService.PostAsync("/api/order/cancel", request);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error canceling order: {ex.Message}");
                throw;
            }
        }

        public async Task SubscribeToNotificationsAsync(int orderId, string email, string phone)
        {
            try
            {
                var request = new { OrderId = orderId, Email = email, Phone = phone };
                await _apiService.PostAsync("/api/order/subscribe-notifications", request);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error subscribing to notifications: {ex.Message}");
                throw;
            }
        }

        private List<TrackingUpdateViewModel> GenerateMockTrackingUpdates(int orderId)
        {
            // This is mock data for demonstration purposes
            // In a real application, this would come from the API
            return new List<TrackingUpdateViewModel>
            {
                new TrackingUpdateViewModel
                {
                    Id = 1,
                    OrderId = orderId,
                    Status = "OrderPlaced",
                    Description = "سفارش شما با موفقیت ثبت شد",
                    UpdateDate = DateTime.Now.AddDays(-3),
                    Location = "فروشگاه آنلاین",
                    UpdatedBy = "سیستم",
                    IsImportant = true
                },
                new TrackingUpdateViewModel
                {
                    Id = 2,
                    OrderId = orderId,
                    Status = "PaymentConfirmed",
                    Description = "پرداخت شما تأیید شد",
                    UpdateDate = DateTime.Now.AddDays(-3).AddMinutes(15),
                    Location = "درگاه پرداخت",
                    UpdatedBy = "سیستم",
                    IsImportant = true
                },
                new TrackingUpdateViewModel
                {
                    Id = 3,
                    OrderId = orderId,
                    Status = "Processing",
                    Description = "سفارش شما در حال پردازش است",
                    UpdateDate = DateTime.Now.AddDays(-2),
                    Location = "انبار مرکزی",
                    UpdatedBy = "تیم پردازش",
                    IsImportant = false
                },
                new TrackingUpdateViewModel
                {
                    Id = 4,
                    OrderId = orderId,
                    Status = "Packaged",
                    Description = "سفارش شما بسته‌بندی شد",
                    UpdateDate = DateTime.Now.AddDays(-1),
                    Location = "انبار مرکزی",
                    UpdatedBy = "تیم بسته‌بندی",
                    IsImportant = false
                },
                new TrackingUpdateViewModel
                {
                    Id = 5,
                    OrderId = orderId,
                    Status = "Shipped",
                    Description = "سفارش شما از انبار خارج شد",
                    UpdateDate = DateTime.Now.AddHours(-12),
                    Location = "پست پیشتاز",
                    UpdatedBy = "پست پیشتاز",
                    IsImportant = true
                },
                new TrackingUpdateViewModel
                {
                    Id = 6,
                    OrderId = orderId,
                    Status = "OutForDelivery",
                    Description = "سفارش شما در حال تحویل است",
                    UpdateDate = DateTime.Now.AddHours(-2),
                    Location = "مرکز توزیع منطقه",
                    UpdatedBy = "پیک توزیع",
                    IsImportant = true
                }
            };
        }
    }
}