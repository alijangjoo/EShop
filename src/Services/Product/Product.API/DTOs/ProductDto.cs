namespace Product.API.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NamePersian { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionPersian { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryNamePersian { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string BrandPersian { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Tags { get; set; } = string.Empty;
    public string TagsPersian { get; set; } = string.Empty;
    public double Weight { get; set; }
    public string Dimensions { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string ColorPersian { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int OrderCount { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsOnSale { get; set; }
    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }
    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductAttributeDto> Attributes { get; set; } = new();
}