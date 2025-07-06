using Payment.API.DTOs;
using Payment.API.Entities;

namespace Payment.API.Services;

public interface IPaymentService
{
    Task<PaymentResultDto> ProcessPaymentAsync(CreatePaymentDto createPaymentDto);
    Task<PaymentResultDto> ProcessIPGPaymentAsync(CreatePaymentDto createPaymentDto);
    Task<PaymentResultDto> ProcessCashPaymentAsync(CreatePaymentDto createPaymentDto);
    Task<PaymentResultDto> VerifyPaymentAsync(string paymentNumber, string transactionId);
    Task<PaymentDto?> GetPaymentByIdAsync(Guid id);
    Task<PaymentDto?> GetPaymentByNumberAsync(string paymentNumber);
    Task<PaymentDto?> GetPaymentByOrderIdAsync(Guid orderId);
    Task<PaginatedPaymentDto> GetPaymentsAsync(PaymentFilterDto filter);
    Task<PaymentResultDto> UpdatePaymentStatusAsync(Guid id, UpdatePaymentDto updatePaymentDto);
    Task<PaymentResultDto> RefundPaymentAsync(Guid id, string reason, string reasonPersian);
    Task<PaymentResultDto> CancelPaymentAsync(Guid id, string reason, string reasonPersian);
    Task<List<PaymentDto>> GetPaymentsByUserAsync(string userName);
    Task<decimal> GetTotalPaymentsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<Dictionary<PaymentStatus, int>> GetPaymentStatusStatsAsync();
}