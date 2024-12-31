using Microsoft.EntityFrameworkCore;
using Product.API.Data;
using Product.API.Models;

namespace Product.API.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Models.Product>> GetProductsAsync(
        int page = 1,
        int pageSize = 10,
        string? category = null,
        string? search = null,
        string? sortBy = null,
        string? sortOrder = null)
    {
        var query = _context.Products.AsQueryable();

        // Apply category filter
        if (!string.IsNullOrWhiteSpace(category))
        {
            var categoryEntity = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == category.ToLower());
            if (categoryEntity != null)
            {
                query = query.Where(p => p.CategoryId == categoryEntity.Id);
            }
        }

        // Apply search
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search));
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var isDescending = sortOrder?.ToLower() == "desc";
            query = sortBy.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "price" => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "createdat" => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt);
        }

        // Apply pagination
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return products;
    }

    public async Task<Models.Product?> GetProductByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Models.Product> CreateProductAsync(Models.Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Models.Product> UpdateProductAsync(Models.Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
