using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification.API.DTOs;
using Notification.API.Services;

namespace Notification.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(IEmailService emailService, ISmsService smsService, ILogger<NotificationController> logger)
    {
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    /// <summary>
    /// Send email notification
    /// </summary>
    /// <param name="request">Email request</param>
    /// <returns>Notification result</returns>
    [HttpPost("email")]
    [Authorize]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.To) || string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Body))
            {
                return BadRequest("Email address, subject, and body are required.");
            }

            var result = await _emailService.SendEmailAsync(request);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");
            return StatusCode(500, new NotificationResult
            {
                IsSuccess = false,
                Message = "Internal server error",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// Send SMS notification
    /// </summary>
    /// <param name="request">SMS request</param>
    /// <returns>Notification result</returns>
    [HttpPost("sms")]
    [Authorize]
    public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.To) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Phone number and message are required.");
            }

            var result = await _smsService.SendSmsAsync(request);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS");
            return StatusCode(500, new NotificationResult
            {
                IsSuccess = false,
                Message = "Internal server error",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// Send order confirmation email
    /// </summary>
    /// <param name="toEmail">Customer email</param>
    /// <param name="toName">Customer name</param>
    /// <param name="orderNumber">Order number</param>
    /// <param name="totalAmount">Total amount</param>
    /// <param name="paymentMethod">Payment method</param>
    /// <returns>Notification result</returns>
    [HttpPost("order-confirmation/email")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendOrderConfirmationEmail(
        [FromQuery] string toEmail,
        [FromQuery] string toName,
        [FromQuery] string orderNumber,
        [FromQuery] decimal totalAmount,
        [FromQuery] string paymentMethod)
    {
        try
        {
            var result = await _emailService.SendOrderConfirmationEmailAsync(toEmail, toName, orderNumber, totalAmount, paymentMethod);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending order confirmation email");
            return StatusCode(500, new NotificationResult
            {
                IsSuccess = false,
                Message = "Internal server error",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// Send order confirmation SMS
    /// </summary>
    /// <param name="phoneNumber">Customer phone number</param>
    /// <param name="orderNumber">Order number</param>
    /// <param name="totalAmount">Total amount</param>
    /// <returns>Notification result</returns>
    [HttpPost("order-confirmation/sms")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendOrderConfirmationSms(
        [FromQuery] string phoneNumber,
        [FromQuery] string orderNumber,
        [FromQuery] decimal totalAmount)
    {
        try
        {
            var result = await _smsService.SendOrderConfirmationSmsAsync(phoneNumber, orderNumber, totalAmount);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending order confirmation SMS");
            return StatusCode(500, new NotificationResult
            {
                IsSuccess = false,
                Message = "Internal server error",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// Send payment confirmation email
    /// </summary>
    /// <param name="toEmail">Customer email</param>
    /// <param name="toName">Customer name</param>
    /// <param name="orderNumber">Order number</param>
    /// <param name="amount">Payment amount</param>
    /// <param name="paymentMethod">Payment method</param>
    /// <returns>Notification result</returns>
    [HttpPost("payment-confirmation/email")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendPaymentConfirmationEmail(
        [FromQuery] string toEmail,
        [FromQuery] string toName,
        [FromQuery] string orderNumber,
        [FromQuery] decimal amount,
        [FromQuery] string paymentMethod)
    {
        try
        {
            var result = await _emailService.SendPaymentConfirmationEmailAsync(toEmail, toName, orderNumber, amount, paymentMethod);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending payment confirmation email");
            return StatusCode(500, new NotificationResult
            {
                IsSuccess = false,
                Message = "Internal server error",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// Send payment confirmation SMS
    /// </summary>
    /// <param name="phoneNumber">Customer phone number</param>
    /// <param name="orderNumber">Order number</param>
    /// <param name="amount">Payment amount</param>
    /// <returns>Notification result</returns>
    [HttpPost("payment-confirmation/sms")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendPaymentConfirmationSms(
        [FromQuery] string phoneNumber,
        [FromQuery] string orderNumber,
        [FromQuery] decimal amount)
    {
        try
        {
            var result = await _smsService.SendPaymentConfirmationSmsAsync(phoneNumber, orderNumber, amount);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending payment confirmation SMS");
            return StatusCode(500, new NotificationResult
            {
                IsSuccess = false,
                Message = "Internal server error",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}