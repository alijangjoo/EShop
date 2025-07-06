using EventBus.Messages.Events;
using MassTransit;
using Notification.API.Services;

namespace Notification.API.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<PaymentProcessedConsumer> _logger;

    public PaymentProcessedConsumer(IEmailService emailService, ISmsService smsService, ILogger<PaymentProcessedConsumer> logger)
    {
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var payment = context.Message;
        
        _logger.LogInformation($"Processing payment notification for payment {payment.PaymentNumber}");

        var orderNumber = $"ORD-{payment.OrderId.ToString().Substring(0, 8).ToUpper()}";
        var paymentMethod = payment.PaymentMethod == 1 ? "IPG" : "Cash";

        // We need to get user details from the username or from another source
        // For now, we'll use a placeholder approach
        var customerName = payment.UserName; // This might need to be improved
        var customerEmail = $"{payment.UserName}@example.com"; // This should be retrieved from Identity service
        var customerPhone = "09123456789"; // This should be retrieved from Identity service

        if (payment.Status == 3) // Completed
        {
            // Send payment success notifications
            try
            {
                var emailResult = await _emailService.SendPaymentConfirmationEmailAsync(
                    customerEmail,
                    customerName,
                    orderNumber,
                    payment.Amount,
                    paymentMethod
                );

                if (emailResult.IsSuccess)
                {
                    _logger.LogInformation($"Payment confirmation email sent successfully to {customerEmail}");
                }
                else
                {
                    _logger.LogError($"Failed to send payment confirmation email to {customerEmail}: {emailResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while sending payment confirmation email to {customerEmail}");
            }

            try
            {
                var smsResult = await _smsService.SendPaymentConfirmationSmsAsync(
                    customerPhone,
                    orderNumber,
                    payment.Amount
                );

                if (smsResult.IsSuccess)
                {
                    _logger.LogInformation($"Payment confirmation SMS sent successfully to {customerPhone}");
                }
                else
                {
                    _logger.LogError($"Failed to send payment confirmation SMS to {customerPhone}: {smsResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while sending payment confirmation SMS to {customerPhone}");
            }
        }
        else if (payment.Status == 4) // Failed
        {
            // Send payment failure notifications
            var failureReason = payment.FailureReasonPersian ?? payment.FailureReason ?? "خطای نامشخص";
            
            try
            {
                var emailResult = await _emailService.SendPaymentFailureEmailAsync(
                    customerEmail,
                    customerName,
                    orderNumber,
                    payment.Amount,
                    failureReason
                );

                if (emailResult.IsSuccess)
                {
                    _logger.LogInformation($"Payment failure email sent successfully to {customerEmail}");
                }
                else
                {
                    _logger.LogError($"Failed to send payment failure email to {customerEmail}: {emailResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while sending payment failure email to {customerEmail}");
            }

            try
            {
                var smsResult = await _smsService.SendPaymentFailureSmsAsync(
                    customerPhone,
                    orderNumber,
                    failureReason
                );

                if (smsResult.IsSuccess)
                {
                    _logger.LogInformation($"Payment failure SMS sent successfully to {customerPhone}");
                }
                else
                {
                    _logger.LogError($"Failed to send payment failure SMS to {customerPhone}: {smsResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while sending payment failure SMS to {customerPhone}");
            }
        }

        _logger.LogInformation($"Completed processing payment notification for payment {payment.PaymentNumber}");
    }
}