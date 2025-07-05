using System.ComponentModel.DataAnnotations;

namespace Product.API.DTOs;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public string AltTextPersian { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductImageDto
{
    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(100)]
    public string AltText { get; set; } = string.Empty;

    [StringLength(100)]
    public string AltTextPersian { get; set; } = string.Empty;

    public bool IsMain { get; set; } = false;
    public int SortOrder { get; set; } = 0;
}