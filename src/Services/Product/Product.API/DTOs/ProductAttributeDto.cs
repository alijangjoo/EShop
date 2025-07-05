using System.ComponentModel.DataAnnotations;

namespace Product.API.DTOs;

public class ProductAttributeDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NamePersian { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValuePersian { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductAttributeDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string NamePersian { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Value { get; set; } = string.Empty;

    [StringLength(200)]
    public string ValuePersian { get; set; } = string.Empty;

    public int SortOrder { get; set; } = 0;
}