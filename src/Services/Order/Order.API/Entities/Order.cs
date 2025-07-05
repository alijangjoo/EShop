using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Order.API.Entities;

public class Order
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [Required]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    // Customer Information
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    // Shipping Information
    [Required]
    [StringLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ShippingCity { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ShippingState { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ShippingCountry { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string ShippingZipCode { get; set; } = string.Empty;

    // Payment Information (for IPG)
    [StringLength(100)]
    public string? CardName { get; set; }

    [StringLength(100)]
    public string? CardNumber { get; set; }

    [StringLength(10)]
    public string? CardExpiration { get; set; }

    [StringLength(10)]
    public string? CVV { get; set; }

    // Persian Fields
    [StringLength(100)]
    public string FirstNamePersian { get; set; } = string.Empty;

    [StringLength(100)]
    public string LastNamePersian { get; set; } = string.Empty;

    [StringLength(500)]
    public string ShippingAddressPersian { get; set; } = string.Empty;

    [StringLength(100)]
    public string ShippingCityPersian { get; set; } = string.Empty;

    [StringLength(100)]
    public string ShippingStatePersian { get; set; } = string.Empty;

    // Additional Fields
    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(1000)]
    public string? NotesPersian { get; set; }

    [StringLength(100)]
    public string? TrackingNumber { get; set; }

    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    // Navigation Properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    // Audit Fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string UpdatedBy { get; set; } = string.Empty;
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Returned = 7
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