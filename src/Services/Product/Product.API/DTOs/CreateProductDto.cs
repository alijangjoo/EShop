using System.ComponentModel.DataAnnotations;

namespace Product.API.DTOs;

public class CreateProductDto
{
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
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? DiscountPrice { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public Guid CategoryId { get; set; }

    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [StringLength(100)]
    public string BrandPersian { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

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

    public bool IsFeatured { get; set; } = false;
    public bool IsOnSale { get; set; } = false;

    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }

    public List<CreateProductImageDto> Images { get; set; } = new();
    public List<CreateProductAttributeDto> Attributes { get; set; } = new();
}