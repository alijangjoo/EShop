using EventBus.Messages.Events;
using MassTransit;
using Notification.API.Services;

namespace Notification.API.Consumers;

public class OrderCheckoutConsumer : IConsumer<OrderCheckoutEvent>
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<OrderCheckoutConsumer> _logger;

    public OrderCheckoutConsumer(IEmailService emailService, ISmsService smsService, ILogger<OrderCheckoutConsumer> logger)
    {
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCheckoutEvent> context)
    {
        var order = context.Message;
        
        _logger.LogInformation($"Processing order checkout notification for order {order.Id}");

        var customerName = $"{order.FirstName} {order.LastName}";
        var orderNumber = $"ORD-{order.Id.ToString().Substring(0, 8).ToUpper()}";
        var paymentMethod = order.PaymentMethod == 1 ? "IPG" : "Cash";

        // Send email notification
        try
        {
            var emailResult = await _emailService.SendOrderConfirmationEmailAsync(
                order.EmailAddress,
                customerName,
                orderNumber,
                order.TotalPrice,
                paymentMethod
            );

            if (emailResult.IsSuccess)
            {
                _logger.LogInformation($"Order confirmation email sent successfully to {order.EmailAddress}");
            }
            else
            {
                _logger.LogError($"Failed to send order confirmation email to {order.EmailAddress}: {emailResult.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while sending order confirmation email to {order.EmailAddress}");
        }

        // Send SMS notification
        try
        {
            var smsResult = await _smsService.SendOrderConfirmationSmsAsync(
                order.PhoneNumber,
                orderNumber,
                order.TotalPrice
            );

            if (smsResult.IsSuccess)
            {
                _logger.LogInformation($"Order confirmation SMS sent successfully to {order.PhoneNumber}");
            }
            else
            {
                _logger.LogError($"Failed to send order confirmation SMS to {order.PhoneNumber}: {smsResult.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while sending order confirmation SMS to {order.PhoneNumber}");
        }

        _logger.LogInformation($"Completed processing order checkout notification for order {order.Id}");
    }
}