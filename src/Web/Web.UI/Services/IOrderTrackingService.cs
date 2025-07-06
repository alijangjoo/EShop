using Web.UI.Models;

namespace Web.UI.Services
{
    public interface IOrderTrackingService
    {
        Task<OrderTrackingViewModel> GetOrderTrackingAsync(string orderNumber, string email);
        Task<OrderTrackingViewModel> GetOrderTrackingByIdAsync(int orderId);
        Task<List<OrderTrackingViewModel>> GetUserOrdersAsync();
        Task<List<TrackingUpdateViewModel>> GetTrackingUpdatesAsync(int orderId);
        Task RequestDeliveryUpdateAsync(int orderId, string customerPhone);
        Task CancelOrderAsync(int orderId, string reason);
        Task SubscribeToNotificationsAsync(int orderId, string email, string phone);
    }
}