using Notification.API.DTOs;

namespace Notification.API.Services;

public interface ISmsService
{
    Task<NotificationResult> SendSmsAsync(SmsRequest request);
    Task<NotificationResult> SendOrderConfirmationSmsAsync(string phoneNumber, string orderNumber, decimal totalAmount);
    Task<NotificationResult> SendPaymentConfirmationSmsAsync(string phoneNumber, string orderNumber, decimal amount);
    Task<NotificationResult> SendPaymentFailureSmsAsync(string phoneNumber, string orderNumber, string failureReason);
}