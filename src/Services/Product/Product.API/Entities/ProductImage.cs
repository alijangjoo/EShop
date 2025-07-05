using System.ComponentModel.DataAnnotations;

namespace Product.API.Entities;

public class ProductImage
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(100)]
    public string AltText { get; set; } = string.Empty;

    [StringLength(100)]
    public string AltTextPersian { get; set; } = string.Empty;

    public bool IsMain { get; set; } = false;
    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}