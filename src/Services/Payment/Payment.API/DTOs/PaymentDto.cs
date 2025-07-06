using Payment.API.Entities;

namespace Payment.API.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    
    // Customer Information
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    // Payment Details
    public string? CardName { get; set; }
    public string? CardLastFourDigits { get; set; }
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? BankName { get; set; }
    public string? BankNamePersian { get; set; }
    
    // Timestamps
    public DateTime PaymentDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    // Additional Information
    public string? Description { get; set; }
    public string? DescriptionPersian { get; set; }
    public string? Notes { get; set; }
    public string? NotesPersian { get; set; }
    public string? FailureReason { get; set; }
    public string? FailureReasonPersian { get; set; }
    
    // Audit Fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}

public class CreatePaymentDto
{
    public Guid OrderId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    
    // Customer Information
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    // Payment Details (for IPG)
    public string? CardName { get; set; }
    public string? CardNumber { get; set; }
    public string? Expiration { get; set; }
    public string? CVV { get; set; }
    
    // Additional Information
    public string? Description { get; set; }
    public string? DescriptionPersian { get; set; }
    public string? Notes { get; set; }
    public string? NotesPersian { get; set; }
}

public class UpdatePaymentDto
{
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? BankName { get; set; }
    public string? BankNamePersian { get; set; }
    public string? FailureReason { get; set; }
    public string? FailureReasonPersian { get; set; }
    public string? Notes { get; set; }
    public string? NotesPersian { get; set; }
}

public class PaymentFilterDto
{
    public Guid? OrderId { get; set; }
    public string? UserName { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public PaymentStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "PaymentDate";
    public bool SortDescending { get; set; } = true;
}

public class PaymentResultDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string MessagePersian { get; set; } = string.Empty;
    public PaymentDto? Payment { get; set; }
    public string? RedirectUrl { get; set; }
}

public class PaginatedPaymentDto
{
    public List<PaymentDto> Payments { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}