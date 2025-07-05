using Order.API.Entities;

namespace Order.API.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingState { get; set; } = string.Empty;
    public string ShippingCountry { get; set; } = string.Empty;
    public string ShippingZipCode { get; set; } = string.Empty;
    public string FirstNamePersian { get; set; } = string.Empty;
    public string LastNamePersian { get; set; } = string.Empty;
    public string ShippingAddressPersian { get; set; } = string.Empty;
    public string ShippingCityPersian { get; set; } = string.Empty;
    public string ShippingStatePersian { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? NotesPersian { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNamePersian { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ProductImageUrl { get; set; }
    public string? ProductBrand { get; set; }
    public string? ProductBrandPersian { get; set; }
    public string? ProductSize { get; set; }
    public string? ProductColor { get; set; }
    public string? ProductColorPersian { get; set; }
    public DateTime CreatedAt { get; set; }
}