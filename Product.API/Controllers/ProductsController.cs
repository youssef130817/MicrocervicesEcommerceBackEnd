using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product.API.Data;
using Product.API.Models;
using Product.API.Services;

namespace Product.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ImageService _imageService;

    public ProductsController(AppDbContext context, ImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    private static object FormatProductResponse(Models.Product p)
    {
        return new
        {
            p.Id,
            p.Name,
            p.Description,
            Price = Math.Round(p.Price, 2),
            p.Stock,
            p.CategoryId,
            Category = p.Category == null ? null : new
            {
                p.Category.Id,
                p.Category.Name,
                p.Category.Description
            },
            Images = p.Images.Select(i => new
            {
                i.Id,
                i.ProductId,
                i.ImageUrl
            }),
            p.CreatedAt,
            p.UpdatedAt
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? category = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortOrder = "asc")
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsQueryable();

        // Apply category filter
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category.Name.ToLower() == category.ToLower());
        }

        // Apply search
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "price" => sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                "name" => sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                _ => query.OrderBy(p => p.Name)
            };
        }

        // Apply pagination
        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            products = products.Select(FormatProductResponse),
            pagination = new
            {
                currentPage = page,
                pageSize,
                totalPages,
                totalItems
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(FormatProductResponse(product));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
    {
        var category = await _context.Categories.FindAsync(request.CategoryId);
        if (category == null)
        {
            return BadRequest(new { message = "Invalid category" });
        }

        var product = new Models.Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = Math.Round(request.GetParsedPrice(), 2),
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);

        // Handle images
        if (request.Images != null)
        {
            foreach (var image in request.Images)
            {
                var imageUrl = await _imageService.SaveImageAsync(image);
                product.Images.Add(new ProductImage
                {
                    ImageUrl = imageUrl,
                    ProductId = product.Id
                });
            }
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, FormatProductResponse(product));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] UpdateProductRequest request)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        if (request.CategoryId != null)
        {
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "Invalid category" });
            }
            product.CategoryId = request.CategoryId.Value;
        }

        product.Name = request.Name ?? product.Name;
        product.Description = request.Description ?? product.Description;
        product.Price = request.GetParsedPrice().HasValue ? Math.Round(request.GetParsedPrice().Value, 2) : product.Price;
        product.Stock = request.Stock ?? product.Stock;
        product.UpdatedAt = DateTime.UtcNow;

        // Handle new images
        if (request.Images != null)
        {
            foreach (var image in request.Images)
            {
                var imageUrl = await _imageService.SaveImageAsync(image);
                product.Images.Add(new ProductImage
                {
                    ImageUrl = imageUrl,
                    ProductId = product.Id
                });
            }
        }

        // Remove deleted images
        if (request.DeletedImageIds != null)
        {
            var imagesToDelete = product.Images.Where(i => request.DeletedImageIds.Contains(i.Id)).ToList();
            foreach (var image in imagesToDelete)
            {
                await _imageService.DeleteImageAsync(image.ImageUrl);
                product.Images.Remove(image);
            }
        }

        await _context.SaveChangesAsync();
        return Ok(FormatProductResponse(product));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        // Delete associated images
        foreach (var image in product.Images)
        {
            await _imageService.DeleteImageAsync(image.ImageUrl);
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Price { get; set; } = "0";
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public List<IFormFile>? Images { get; set; }

    public decimal GetParsedPrice()
    {
        if (decimal.TryParse(Price.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
        {
            return Math.Round(result, 2);
        }
        return 0;
    }
}

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Price { get; set; }
    public int? Stock { get; set; }
    public Guid? CategoryId { get; set; }
    public List<IFormFile>? Images { get; set; }
    public List<Guid>? DeletedImageIds { get; set; }

    public decimal? GetParsedPrice()
    {
        if (Price == null) return null;
        if (decimal.TryParse(Price.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
        {
            return Math.Round(result, 2);
        }
        return null;
    }
}
