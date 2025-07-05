using System.ComponentModel.DataAnnotations;
using Order.API.Entities;

namespace Order.API.DTOs;

public class CreateOrderDto
{
    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

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

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(1000)]
    public string? NotesPersian { get; set; }

    [Required]
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}

public class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [StringLength(200)]
    public string ProductNamePersian { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [StringLength(500)]
    public string? ProductImageUrl { get; set; }

    [StringLength(100)]
    public string? ProductBrand { get; set; }

    [StringLength(100)]
    public string? ProductBrandPersian { get; set; }

    [StringLength(50)]
    public string? ProductSize { get; set; }

    [StringLength(50)]
    public string? ProductColor { get; set; }

    [StringLength(50)]
    public string? ProductColorPersian { get; set; }
}