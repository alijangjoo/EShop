using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Order.API.Entities;

public class OrderItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [StringLength(200)]
    public string ProductNamePersian { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

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

    // Navigation Properties
    public Order Order { get; set; } = null!;

    // Audit Fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}