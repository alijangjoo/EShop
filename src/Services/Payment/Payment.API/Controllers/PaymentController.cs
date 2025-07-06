using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.API.DTOs;
using Payment.API.Entities;
using Payment.API.Services;
using System.Security.Claims;

namespace Payment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new payment
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PaymentResultDto>> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
    {
        try
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "User not authenticated",
                    MessagePersian = "کاربر احراز هویت نشده است"
                });
            }

            createPaymentDto.UserName = userName;
            var result = await _paymentService.ProcessPaymentAsync(createPaymentDto);
            
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            return StatusCode(500, new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Internal server error",
                MessagePersian = "خطای داخلی سرور"
            });
        }
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<PaymentDto>> GetPayment(Guid id)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found", messagePersian = "پرداخت یافت نشد" });
            }

            // Check if user has access to this payment
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin && payment.UserName != userName)
            {
                return Forbid();
            }

            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment {PaymentId}", id);
            return StatusCode(500, new { message = "Internal server error", messagePersian = "خطای داخلی سرور" });
        }
    }

    /// <summary>
    /// Get payment by payment number
    /// </summary>
    [HttpGet("number/{paymentNumber}")]
    [Authorize]
    public async Task<ActionResult<PaymentDto>> GetPaymentByNumber(string paymentNumber)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByNumberAsync(paymentNumber);
            
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found", messagePersian = "پرداخت یافت نشد" });
            }

            // Check if user has access to this payment
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin && payment.UserName != userName)
            {
                return Forbid();
            }

            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment by number {PaymentNumber}", paymentNumber);
            return StatusCode(500, new { message = "Internal server error", messagePersian = "خطای داخلی سرور" });
        }
    }

    /// <summary>
    /// Get payment by order ID
    /// </summary>
    [HttpGet("order/{orderId}")]
    [Authorize]
    public async Task<ActionResult<PaymentDto>> GetPaymentByOrderId(Guid orderId)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found", messagePersian = "پرداخت یافت نشد" });
            }

            // Check if user has access to this payment
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin && payment.UserName != userName)
            {
                return Forbid();
            }

            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment by order ID {OrderId}", orderId);
            return StatusCode(500, new { message = "Internal server error", messagePersian = "خطای داخلی سرور" });
        }
    }

    /// <summary>
    /// Get payments with filtering and pagination
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PaginatedPaymentDto>> GetPayments([FromQuery] PaymentFilterDto filter)
    {
        try
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var isAdmin = User.IsInRole("Admin");
            
            // Non-admin users can only see their own payments
            if (!isAdmin)
            {
                filter.UserName = userName;
            }

            var result = await _paymentService.GetPaymentsAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payments");
            return StatusCode(500, new { message = "Internal server error", messagePersian = "خطای داخلی سرور" });
        }
    }

    /// <summary>
    /// Get user's payments
    /// </summary>
    [HttpGet("user")]
    [Authorize]
    public async Task<ActionResult<List<PaymentDto>>> GetUserPayments()
    {
        try
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var payments = await _paymentService.GetPaymentsByUserAsync(userName);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user payments");
            return StatusCode(500, new { message = "Internal server error", messagePersian = "خطای داخلی سرور" });
        }
    }

    /// <summary>
    /// Verify payment (callback from payment gateway)
    /// </summary>
    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<ActionResult<PaymentResultDto>> VerifyPayment([FromBody] VerifyPaymentDto verifyPaymentDto)
    {
        try
        {
            var result = await _paymentService.VerifyPaymentAsync(verifyPaymentDto.PaymentNumber, verifyPaymentDto.TransactionId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment");
            return StatusCode(500, new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Internal server error",
                MessagePersian = "خطای داخلی سرور"
            });
        }
    }

    /// <summary>
    /// Update payment status (Admin only)
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaymentResultDto>> UpdatePaymentStatus(Guid id, [FromBody] UpdatePaymentDto updatePaymentDto)
    {
        try
        {
            var result = await _paymentService.UpdatePaymentStatusAsync(id, updatePaymentDto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status {PaymentId}", id);
            return StatusCode(500, new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Internal server error",
                MessagePersian = "خطای داخلی سرور"
            });
        }
    }

    /// <summary>
    /// Refund payment (Admin only)
    /// </summary>
    [HttpPost("{id}/refund")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaymentResultDto>> RefundPayment(Guid id, [FromBody] RefundPaymentDto refundDto)
    {
        try
        {
            var result = await _paymentService.RefundPaymentAsync(id, refundDto.Reason, refundDto.ReasonPersian);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment {PaymentId}", id);
            return StatusCode(500, new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Internal server error",
                MessagePersian = "خطای داخلی سرور"
            });
        }
    }

    /// <summary>
    /// Cancel payment (Admin only)
    /// </summary>
    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaymentResultDto>> CancelPayment(Guid id, [FromBody] CancelPaymentDto cancelDto)
    {
        try
        {
            var result = await _paymentService.CancelPaymentAsync(id, cancelDto.Reason, cancelDto.ReasonPersian);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling payment {PaymentId}", id);
            return StatusCode(500, new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Internal server error",
                MessagePersian = "خطای داخلی سرور"
            });
        }
    }

    /// <summary>
    /// Get payment statistics (Admin only)
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> GetPaymentStats()
    {
        try
        {
            var statusStats = await _paymentService.GetPaymentStatusStatsAsync();
            var totalToday = await _paymentService.GetTotalPaymentsAsync(DateTime.Today, DateTime.Today.AddDays(1));
            var totalThisMonth = await _paymentService.GetTotalPaymentsAsync(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today.AddDays(1));
            var totalAll = await _paymentService.GetTotalPaymentsAsync();

            return Ok(new
            {
                StatusStats = statusStats,
                TotalToday = totalToday,
                TotalThisMonth = totalThisMonth,
                TotalAll = totalAll
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment stats");
            return StatusCode(500, new { message = "Internal server error", messagePersian = "خطای داخلی سرور" });
        }
    }
}

// Additional DTOs for API operations
public class VerifyPaymentDto
{
    public string PaymentNumber { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}

public class RefundPaymentDto
{
    public string Reason { get; set; } = string.Empty;
    public string ReasonPersian { get; set; } = string.Empty;
}

public class CancelPaymentDto
{
    public string Reason { get; set; } = string.Empty;
    public string ReasonPersian { get; set; } = string.Empty;
}