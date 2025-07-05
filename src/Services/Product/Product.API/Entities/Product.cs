using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Product.API.Entities;

public class Product
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string NamePersian { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(1000)]
    public string DescriptionPersian { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public int Stock { get; set; }

    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [StringLength(100)]
    public string BrandPersian { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(50)]
    public string UpdatedBy { get; set; } = string.Empty;

    // Additional properties for Persian e-commerce
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountPrice { get; set; }

    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }

    [StringLength(500)]
    public string Tags { get; set; } = string.Empty;

    [StringLength(500)]
    public string TagsPersian { get; set; } = string.Empty;

    public double Weight { get; set; }

    [StringLength(100)]
    public string Dimensions { get; set; } = string.Empty;

    [StringLength(100)]
    public string Color { get; set; } = string.Empty;

    [StringLength(100)]
    public string ColorPersian { get; set; } = string.Empty;

    [StringLength(50)]
    public string Size { get; set; } = string.Empty;

    public int ViewCount { get; set; } = 0;
    public int OrderCount { get; set; } = 0;

    public bool IsFeatured { get; set; } = false;
    public bool IsOnSale { get; set; } = false;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
}