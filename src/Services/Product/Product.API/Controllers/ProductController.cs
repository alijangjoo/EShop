using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Product.API.Data;
using Product.API.DTOs;
using Product.API.Entities;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductController> _logger;

    public ProductController(ProductDbContext context, IMapper mapper, ILogger<ProductController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? brand = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? featured = null,
        [FromQuery] bool? onSale = null,
        [FromQuery] string sortBy = "name",
        [FromQuery] string sortOrder = "asc")
    {
        try
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Attributes)
                .Where(p => p.IsActive);

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || 
                                   p.NamePersian.Contains(search) ||
                                   p.Description.Contains(search) ||
                                   p.DescriptionPersian.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(p => p.Brand.Contains(brand) || p.BrandPersian.Contains(brand));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (featured.HasValue)
            {
                query = query.Where(p => p.IsFeatured == featured.Value);
            }

            if (onSale.HasValue)
            {
                query = query.Where(p => p.IsOnSale == onSale.Value);
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "name" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "price" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "created" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                "views" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.ViewCount) : query.OrderBy(p => p.ViewCount),
                _ => query.OrderBy(p => p.Name)
            };

            var totalItems = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductDto>>(products);

            Response.Headers.Add("X-Total-Count", totalItems.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());

            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Attributes)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            // Increment view count
            product.ViewCount++;
            await _context.SaveChangesAsync();

            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product with ID: {ProductId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Entities.Product>(createProductDto);
            product.Id = Guid.NewGuid();
            product.CreatedBy = User.Identity?.Name ?? "System";
            product.UpdatedBy = User.Identity?.Name ?? "System";

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productDto = _mapper.Map<ProductDto>(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(Guid id, CreateProductDto updateProductDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _mapper.Map(updateProductDto, product);
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product with ID: {ProductId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Soft delete
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product with ID: {ProductId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts([FromQuery] int count = 10)
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.ViewCount)
                .Take(count)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching featured products");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("on-sale")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetOnSaleProducts([FromQuery] int count = 10)
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsActive && p.IsOnSale)
                .OrderByDescending(p => p.OrderCount)
                .Take(count)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching on-sale products");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Search query is required");
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsActive && 
                           (p.Name.Contains(query) || 
                            p.NamePersian.Contains(query) ||
                            p.Description.Contains(query) ||
                            p.DescriptionPersian.Contains(query) ||
                            p.Tags.Contains(query) ||
                            p.TagsPersian.Contains(query)))
                .OrderByDescending(p => p.ViewCount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching products");
            return StatusCode(500, "Internal server error");
        }
    }
}