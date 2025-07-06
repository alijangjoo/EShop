using Notification.API.DTOs;

namespace Notification.API.Services;

public interface IEmailService
{
    Task<NotificationResult> SendEmailAsync(EmailRequest request);
    Task<NotificationResult> SendOrderConfirmationEmailAsync(string toEmail, string toName, string orderNumber, decimal totalAmount, string paymentMethod);
    Task<NotificationResult> SendPaymentConfirmationEmailAsync(string toEmail, string toName, string orderNumber, decimal amount, string paymentMethod);
    Task<NotificationResult> SendPaymentFailureEmailAsync(string toEmail, string toName, string orderNumber, decimal amount, string failureReason);
}