using System.ComponentModel.DataAnnotations;

namespace Product.API.Entities;

public class Category
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string NamePersian { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(500)]
    public string DescriptionPersian { get; set; } = string.Empty;

    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }

    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();

    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(50)]
    public string UpdatedBy { get; set; } = string.Empty;
}