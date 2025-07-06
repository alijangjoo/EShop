using Microsoft.EntityFrameworkCore;
using Payment.API.Data;
using Payment.API.DTOs;
using Payment.API.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace Payment.API.Services;

public class PaymentService : IPaymentService
{
    private readonly PaymentContext _context;
    private readonly ILogger<PaymentService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public PaymentService(
        PaymentContext context,
        ILogger<PaymentService> logger,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<PaymentResultDto> ProcessPaymentAsync(CreatePaymentDto createPaymentDto)
    {
        try
        {
            _logger.LogInformation("Processing payment for Order: {OrderId}, Method: {PaymentMethod}", 
                createPaymentDto.OrderId, createPaymentDto.PaymentMethod);

            return createPaymentDto.PaymentMethod switch
            {
                PaymentMethod.IPG => await ProcessIPGPaymentAsync(createPaymentDto),
                PaymentMethod.Cash => await ProcessCashPaymentAsync(createPaymentDto),
                _ => new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "Invalid payment method",
                    MessagePersian = "روش پرداخت نامعتبر است"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for Order: {OrderId}", createPaymentDto.OrderId);
            return new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Payment processing failed",
                MessagePersian = "خطا در پردازش پرداخت"
            };
        }
    }

    public async Task<PaymentResultDto> ProcessIPGPaymentAsync(CreatePaymentDto createPaymentDto)
    {
        try
        {
            // Create payment record
            var payment = new Entities.Payment
            {
                Id = Guid.NewGuid(),
                PaymentNumber = GeneratePaymentNumber(),
                OrderId = createPaymentDto.OrderId,
                UserName = createPaymentDto.UserName,
                Amount = createPaymentDto.Amount,
                PaymentMethod = PaymentMethod.IPG,
                Status = PaymentStatus.Pending,
                CustomerName = createPaymentDto.CustomerName,
                CustomerEmail = createPaymentDto.CustomerEmail,
                CustomerPhone = createPaymentDto.CustomerPhone,
                CardName = createPaymentDto.CardName,
                CardLastFourDigits = createPaymentDto.CardNumber?.Length >= 4 ? 
                    createPaymentDto.CardNumber.Substring(createPaymentDto.CardNumber.Length - 4) : null,
                Description = createPaymentDto.Description,
                DescriptionPersian = createPaymentDto.DescriptionPersian,
                Notes = createPaymentDto.Notes,
                NotesPersian = createPaymentDto.NotesPersian,
                CreatedBy = GetCurrentUser(),
                UpdatedBy = GetCurrentUser()
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Simulate IPG processing (In real scenario, integrate with payment gateway)
            var ipgResult = await SimulateIPGProcessing(payment, createPaymentDto);
            
            if (ipgResult.IsSuccess)
            {
                payment.Status = PaymentStatus.Processing;
                payment.ProcessedDate = DateTime.UtcNow;
                payment.TransactionId = ipgResult.TransactionId;
                payment.GatewayTransactionId = ipgResult.GatewayTransactionId;
                payment.UpdatedBy = GetCurrentUser();
                payment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new PaymentResultDto
                {
                    IsSuccess = true,
                    Message = "Payment initiated successfully",
                    MessagePersian = "پرداخت با موفقیت آغاز شد",
                    Payment = MapToDto(payment),
                    RedirectUrl = ipgResult.RedirectUrl
                };
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = ipgResult.ErrorMessage;
                payment.FailureReasonPersian = ipgResult.ErrorMessagePersian;
                payment.UpdatedBy = GetCurrentUser();
                payment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = ipgResult.ErrorMessage,
                    MessagePersian = ipgResult.ErrorMessagePersian,
                    Payment = MapToDto(payment)
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing IPG payment for Order: {OrderId}", createPaymentDto.OrderId);
            return new PaymentResultDto
            {
                IsSuccess = false,
                Message = "IPG payment processing failed",
                MessagePersian = "خطا در پردازش پرداخت درگاه"
            };
        }
    }

    public async Task<PaymentResultDto> ProcessCashPaymentAsync(CreatePaymentDto createPaymentDto)
    {
        try
        {
            // Create payment record for cash payment
            var payment = new Entities.Payment
            {
                Id = Guid.NewGuid(),
                PaymentNumber = GeneratePaymentNumber(),
                OrderId = createPaymentDto.OrderId,
                UserName = createPaymentDto.UserName,
                Amount = createPaymentDto.Amount,
                PaymentMethod = PaymentMethod.Cash,
                Status = PaymentStatus.Pending,
                CustomerName = createPaymentDto.CustomerName,
                CustomerEmail = createPaymentDto.CustomerEmail,
                CustomerPhone = createPaymentDto.CustomerPhone,
                Description = createPaymentDto.Description ?? "Cash on Delivery",
                DescriptionPersian = createPaymentDto.DescriptionPersian ?? "پرداخت نقدی هنگام تحویل",
                Notes = createPaymentDto.Notes,
                NotesPersian = createPaymentDto.NotesPersian,
                CreatedBy = GetCurrentUser(),
                UpdatedBy = GetCurrentUser()
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cash payment created for Order: {OrderId}, Payment: {PaymentNumber}", 
                createPaymentDto.OrderId, payment.PaymentNumber);

            return new PaymentResultDto
            {
                IsSuccess = true,
                Message = "Cash payment registered successfully",
                MessagePersian = "پرداخت نقدی با موفقیت ثبت شد",
                Payment = MapToDto(payment)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing cash payment for Order: {OrderId}", createPaymentDto.OrderId);
            return new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Cash payment processing failed",
                MessagePersian = "خطا در ثبت پرداخت نقدی"
            };
        }
    }

    public async Task<PaymentResultDto> VerifyPaymentAsync(string paymentNumber, string transactionId)
    {
        try
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentNumber == paymentNumber);

            if (payment == null)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "Payment not found",
                    MessagePersian = "پرداخت یافت نشد"
                };
            }

            if (payment.PaymentMethod == PaymentMethod.IPG)
            {
                // Simulate IPG verification (In real scenario, call payment gateway verify API)
                var verificationResult = await SimulateIPGVerification(payment, transactionId);
                
                if (verificationResult.IsSuccess)
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.CompletedDate = DateTime.UtcNow;
                    payment.ReferenceNumber = verificationResult.ReferenceNumber;
                    payment.BankName = verificationResult.BankName;
                    payment.BankNamePersian = verificationResult.BankNamePersian;
                    payment.UpdatedBy = GetCurrentUser();
                    payment.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    return new PaymentResultDto
                    {
                        IsSuccess = true,
                        Message = "Payment verified successfully",
                        MessagePersian = "پرداخت با موفقیت تایید شد",
                        Payment = MapToDto(payment)
                    };
                }
                else
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.FailureReason = verificationResult.ErrorMessage;
                    payment.FailureReasonPersian = verificationResult.ErrorMessagePersian;
                    payment.UpdatedBy = GetCurrentUser();
                    payment.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    return new PaymentResultDto
                    {
                        IsSuccess = false,
                        Message = verificationResult.ErrorMessage,
                        MessagePersian = verificationResult.ErrorMessagePersian,
                        Payment = MapToDto(payment)
                    };
                }
            }
            else
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "Cash payments cannot be verified through gateway",
                    MessagePersian = "پرداخت نقدی قابل تایید از طریق درگاه نیست"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment: {PaymentNumber}", paymentNumber);
            return new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Payment verification failed",
                MessagePersian = "خطا در تایید پرداخت"
            };
        }
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(Guid id)
    {
        var payment = await _context.Payments.FindAsync(id);
        return payment != null ? MapToDto(payment) : null;
    }

    public async Task<PaymentDto?> GetPaymentByNumberAsync(string paymentNumber)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentNumber == paymentNumber);
        return payment != null ? MapToDto(payment) : null;
    }

    public async Task<PaymentDto?> GetPaymentByOrderIdAsync(Guid orderId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
        return payment != null ? MapToDto(payment) : null;
    }

    public async Task<PaginatedPaymentDto> GetPaymentsAsync(PaymentFilterDto filter)
    {
        var query = _context.Payments.AsQueryable();

        // Apply filters
        if (filter.OrderId.HasValue)
            query = query.Where(p => p.OrderId == filter.OrderId.Value);

        if (!string.IsNullOrEmpty(filter.UserName))
            query = query.Where(p => p.UserName.Contains(filter.UserName));

        if (filter.PaymentMethod.HasValue)
            query = query.Where(p => p.PaymentMethod == filter.PaymentMethod.Value);

        if (filter.Status.HasValue)
            query = query.Where(p => p.Status == filter.Status.Value);

        if (filter.FromDate.HasValue)
            query = query.Where(p => p.PaymentDate >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(p => p.PaymentDate <= filter.ToDate.Value);

        if (filter.MinAmount.HasValue)
            query = query.Where(p => p.Amount >= filter.MinAmount.Value);

        if (filter.MaxAmount.HasValue)
            query = query.Where(p => p.Amount <= filter.MaxAmount.Value);

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(p => 
                p.PaymentNumber.Contains(filter.SearchTerm) ||
                p.CustomerName.Contains(filter.SearchTerm) ||
                p.CustomerEmail.Contains(filter.SearchTerm) ||
                p.CustomerPhone.Contains(filter.SearchTerm) ||
                (p.Description != null && p.Description.Contains(filter.SearchTerm)) ||
                (p.DescriptionPersian != null && p.DescriptionPersian.Contains(filter.SearchTerm)));
        }

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "paymentdate" => filter.SortDescending ? query.OrderByDescending(p => p.PaymentDate) : query.OrderBy(p => p.PaymentDate),
            "amount" => filter.SortDescending ? query.OrderByDescending(p => p.Amount) : query.OrderBy(p => p.Amount),
            "status" => filter.SortDescending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
            "paymentmethod" => filter.SortDescending ? query.OrderByDescending(p => p.PaymentMethod) : query.OrderBy(p => p.PaymentMethod),
            _ => query.OrderByDescending(p => p.PaymentDate)
        };

        var totalCount = await query.CountAsync();
        var payments = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PaginatedPaymentDto
        {
            Payments = payments.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<PaymentResultDto> UpdatePaymentStatusAsync(Guid id, UpdatePaymentDto updatePaymentDto)
    {
        try
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "Payment not found",
                    MessagePersian = "پرداخت یافت نشد"
                };
            }

            payment.Status = updatePaymentDto.Status;
            payment.TransactionId = updatePaymentDto.TransactionId ?? payment.TransactionId;
            payment.ReferenceNumber = updatePaymentDto.ReferenceNumber ?? payment.ReferenceNumber;
            payment.GatewayTransactionId = updatePaymentDto.GatewayTransactionId ?? payment.GatewayTransactionId;
            payment.BankName = updatePaymentDto.BankName ?? payment.BankName;
            payment.BankNamePersian = updatePaymentDto.BankNamePersian ?? payment.BankNamePersian;
            payment.FailureReason = updatePaymentDto.FailureReason ?? payment.FailureReason;
            payment.FailureReasonPersian = updatePaymentDto.FailureReasonPersian ?? payment.FailureReasonPersian;
            payment.Notes = updatePaymentDto.Notes ?? payment.Notes;
            payment.NotesPersian = updatePaymentDto.NotesPersian ?? payment.NotesPersian;
            payment.UpdatedBy = GetCurrentUser();
            payment.UpdatedAt = DateTime.UtcNow;

            if (updatePaymentDto.Status == PaymentStatus.Completed)
                payment.CompletedDate = DateTime.UtcNow;
            else if (updatePaymentDto.Status == PaymentStatus.Processing)
                payment.ProcessedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PaymentResultDto
            {
                IsSuccess = true,
                Message = "Payment status updated successfully",
                MessagePersian = "وضعیت پرداخت با موفقیت به‌روزرسانی شد",
                Payment = MapToDto(payment)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status: {PaymentId}", id);
            return new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Payment status update failed",
                MessagePersian = "خطا در به‌روزرسانی وضعیت پرداخت"
            };
        }
    }

    public async Task<PaymentResultDto> RefundPaymentAsync(Guid id, string reason, string reasonPersian)
    {
        try
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "Payment not found",
                    MessagePersian = "پرداخت یافت نشد"
                };
            }

            if (payment.Status != PaymentStatus.Completed)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "Only completed payments can be refunded",
                    MessagePersian = "فقط پرداخت‌های تکمیل شده قابل استرداد هستند"
                };
            }

            payment.Status = PaymentStatus.Refunded;
            payment.FailureReason = reason;
            payment.FailureReasonPersian = reasonPersian;
            payment.UpdatedBy = GetCurrentUser();
            payment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PaymentResultDto
            {
                IsSuccess = true,
                Message = "Payment refunded successfully",
                MessagePersian = "پرداخت با موفقیت استرداد شد",
                Payment = MapToDto(payment)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment: {PaymentId}", id);
            return new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Payment refund failed",
                MessagePersian = "خطا در استرداد پرداخت"
            };
        }
    }

    public async Task<PaymentResultDto> CancelPaymentAsync(Guid id, string reason, string reasonPersian)
    {
        try
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "Payment not found",
                    MessagePersian = "پرداخت یافت نشد"
                };
            }

            if (payment.Status == PaymentStatus.Completed)
            {
                return new PaymentResultDto
                {
                    IsSuccess = false,
                    Message = "Completed payments cannot be cancelled",
                    MessagePersian = "پرداخت‌های تکمیل شده قابل لغو نیستند"
                };
            }

            payment.Status = PaymentStatus.Cancelled;
            payment.FailureReason = reason;
            payment.FailureReasonPersian = reasonPersian;
            payment.UpdatedBy = GetCurrentUser();
            payment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PaymentResultDto
            {
                IsSuccess = true,
                Message = "Payment cancelled successfully",
                MessagePersian = "پرداخت با موفقیت لغو شد",
                Payment = MapToDto(payment)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling payment: {PaymentId}", id);
            return new PaymentResultDto
            {
                IsSuccess = false,
                Message = "Payment cancellation failed",
                MessagePersian = "خطا در لغو پرداخت"
            };
        }
    }

    public async Task<List<PaymentDto>> GetPaymentsByUserAsync(string userName)
    {
        var payments = await _context.Payments
            .Where(p => p.UserName == userName)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        return payments.Select(MapToDto).ToList();
    }

    public async Task<decimal> GetTotalPaymentsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.Payments.Where(p => p.Status == PaymentStatus.Completed);

        if (fromDate.HasValue)
            query = query.Where(p => p.PaymentDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(p => p.PaymentDate <= toDate.Value);

        return await query.SumAsync(p => p.Amount);
    }

    public async Task<Dictionary<PaymentStatus, int>> GetPaymentStatusStatsAsync()
    {
        return await _context.Payments
            .GroupBy(p => p.Status)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    private string GeneratePaymentNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"PAY-{timestamp}-{random}";
    }

    private string GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "System";
    }

    private PaymentDto MapToDto(Entities.Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            OrderId = payment.OrderId,
            UserName = payment.UserName,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            CustomerName = payment.CustomerName,
            CustomerEmail = payment.CustomerEmail,
            CustomerPhone = payment.CustomerPhone,
            CardName = payment.CardName,
            CardLastFourDigits = payment.CardLastFourDigits,
            TransactionId = payment.TransactionId,
            ReferenceNumber = payment.ReferenceNumber,
            GatewayTransactionId = payment.GatewayTransactionId,
            BankName = payment.BankName,
            BankNamePersian = payment.BankNamePersian,
            PaymentDate = payment.PaymentDate,
            ProcessedDate = payment.ProcessedDate,
            CompletedDate = payment.CompletedDate,
            Description = payment.Description,
            DescriptionPersian = payment.DescriptionPersian,
            Notes = payment.Notes,
            NotesPersian = payment.NotesPersian,
            FailureReason = payment.FailureReason,
            FailureReasonPersian = payment.FailureReasonPersian,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt,
            CreatedBy = payment.CreatedBy,
            UpdatedBy = payment.UpdatedBy
        };
    }

    // Simulation methods for IPG processing (Replace with actual gateway integration)
    private async Task<IPGProcessingResult> SimulateIPGProcessing(Entities.Payment payment, CreatePaymentDto createPaymentDto)
    {
        // Simulate processing delay
        await Task.Delay(100);

        // Simulate 90% success rate
        var random = new Random();
        if (random.NextDouble() < 0.9)
        {
            return new IPGProcessingResult
            {
                IsSuccess = true,
                TransactionId = $"TXN-{DateTime.UtcNow.Ticks}",
                GatewayTransactionId = $"GTW-{random.Next(100000, 999999)}",
                RedirectUrl = $"https://gateway.example.com/payment/{payment.PaymentNumber}"
            };
        }
        else
        {
            return new IPGProcessingResult
            {
                IsSuccess = false,
                ErrorMessage = "Payment gateway error",
                ErrorMessagePersian = "خطا در درگاه پرداخت"
            };
        }
    }

    private async Task<IPGVerificationResult> SimulateIPGVerification(Entities.Payment payment, string transactionId)
    {
        // Simulate verification delay
        await Task.Delay(100);

        // Simulate 95% success rate for verification
        var random = new Random();
        if (random.NextDouble() < 0.95)
        {
            return new IPGVerificationResult
            {
                IsSuccess = true,
                ReferenceNumber = $"REF-{DateTime.UtcNow.Ticks}",
                BankName = "Sample Bank",
                BankNamePersian = "بانک نمونه"
            };
        }
        else
        {
            return new IPGVerificationResult
            {
                IsSuccess = false,
                ErrorMessage = "Payment verification failed",
                ErrorMessagePersian = "تایید پرداخت ناموفق"
            };
        }
    }

    private class IPGProcessingResult
    {
        public bool IsSuccess { get; set; }
        public string? TransactionId { get; set; }
        public string? GatewayTransactionId { get; set; }
        public string? RedirectUrl { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorMessagePersian { get; set; }
    }

    private class IPGVerificationResult
    {
        public bool IsSuccess { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankNamePersian { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorMessagePersian { get; set; }
    }
}