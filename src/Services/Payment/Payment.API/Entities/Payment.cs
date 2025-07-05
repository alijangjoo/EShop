using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment.API.Entities;

public class Payment
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string PaymentNumber { get; set; } = string.Empty;

    [Required]
    public Guid OrderId { get; set; }

    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [Required]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    // Customer Information
    [Required]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string CustomerPhone { get; set; } = string.Empty;

    // Payment Details (for IPG)
    [StringLength(100)]
    public string? CardName { get; set; }

    [StringLength(4)]
    public string? CardLastFourDigits { get; set; }

    [StringLength(50)]
    public string? TransactionId { get; set; }

    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    [StringLength(100)]
    public string? GatewayTransactionId { get; set; }

    [StringLength(50)]
    public string? BankName { get; set; }

    [StringLength(50)]
    public string? BankNamePersian { get; set; }

    // Timestamps
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    // Additional Information
    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? DescriptionPersian { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(1000)]
    public string? NotesPersian { get; set; }

    [StringLength(500)]
    public string? FailureReason { get; set; }

    [StringLength(500)]
    public string? FailureReasonPersian { get; set; }

    // Audit Fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string UpdatedBy { get; set; } = string.Empty;
}

public enum PaymentMethod
{
    IPG = 1,
    Cash = 2
}

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    Refunded = 6
}