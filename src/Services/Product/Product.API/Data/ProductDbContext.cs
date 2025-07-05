using Microsoft.EntityFrameworkCore;
using Product.API.Entities;

namespace Product.API.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Entities.Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductAttribute> ProductAttributes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Entities.Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NamePersian).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DescriptionPersian).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiscountPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.BrandPersian).HasMaxLength(100);
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.Property(e => e.TagsPersian).HasMaxLength(500);
            entity.Property(e => e.Dimensions).HasMaxLength(100);
            entity.Property(e => e.Color).HasMaxLength(100);
            entity.Property(e => e.ColorPersian).HasMaxLength(100);
            entity.Property(e => e.Size).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            // Relationships
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            entity.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);

            entity.HasMany(p => p.Attributes)
                .WithOne(a => a.Product)
                .HasForeignKey(a => a.ProductId);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NamePersian).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DescriptionPersian).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            // Self-referencing relationship
            entity.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId);
        });

        // ProductImage configuration
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.AltText).HasMaxLength(100);
            entity.Property(e => e.AltTextPersian).HasMaxLength(100);
        });

        // ProductAttribute configuration
        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NamePersian).HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ValuePersian).HasMaxLength(200);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed categories
        var electronicsCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            NamePersian = "لوازم الکترونیکی",
            Description = "Electronic devices and accessories",
            DescriptionPersian = "دستگاه‌ها و لوازم جانبی الکترونیکی",
            IsActive = true,
            SortOrder = 1,
            CreatedBy = "System"
        };

        var clothingCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Clothing",
            NamePersian = "پوشاک",
            Description = "Clothing and fashion items",
            DescriptionPersian = "لباس و اقلام مد",
            IsActive = true,
            SortOrder = 2,
            CreatedBy = "System"
        };

        var booksCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Books",
            NamePersian = "کتاب",
            Description = "Books and educational materials",
            DescriptionPersian = "کتاب و مواد آموزشی",
            IsActive = true,
            SortOrder = 3,
            CreatedBy = "System"
        };

        modelBuilder.Entity<Category>().HasData(electronicsCategory, clothingCategory, booksCategory);

        // Seed products
        var product1 = new Entities.Product
        {
            Id = Guid.NewGuid(),
            Name = "Smartphone",
            NamePersian = "گوشی هوشمند",
            Description = "Latest smartphone with advanced features",
            DescriptionPersian = "آخرین گوشی هوشمند با ویژگی‌های پیشرفته",
            Price = 599.99m,
            Stock = 100,
            CategoryId = electronicsCategory.Id,
            Brand = "TechBrand",
            BrandPersian = "تک برند",
            IsActive = true,
            IsFeatured = true,
            IsOnSale = true,
            CreatedBy = "System",
            Tags = "smartphone, mobile, tech",
            TagsPersian = "گوشی هوشمند، موبایل، تکنولوژی"
        };

        var product2 = new Entities.Product
        {
            Id = Guid.NewGuid(),
            Name = "T-Shirt",
            NamePersian = "تی شرت",
            Description = "Comfortable cotton t-shirt",
            DescriptionPersian = "تی شرت پنبه‌ای راحت",
            Price = 19.99m,
            Stock = 200,
            CategoryId = clothingCategory.Id,
            Brand = "FashionBrand",
            BrandPersian = "فشن برند",
            IsActive = true,
            CreatedBy = "System",
            Tags = "tshirt, cotton, clothing",
            TagsPersian = "تی شرت، پنبه، لباس"
        };

        modelBuilder.Entity<Entities.Product>().HasData(product1, product2);
    }
}