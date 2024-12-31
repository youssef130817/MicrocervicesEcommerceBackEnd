using Product.API.Models;

namespace Product.API.Services;

public interface IProductService
{
    Task<IEnumerable<Models.Product>> GetProductsAsync(
        int page = 1,
        int pageSize = 10,
        string? category = null,
        string? search = null,
        string? sortBy = null,
        string? sortOrder = null);
    
    Task<Models.Product?> GetProductByIdAsync(Guid id);
    Task<Models.Product> CreateProductAsync(Models.Product product);
    Task<Models.Product> UpdateProductAsync(Models.Product product);
    Task DeleteProductAsync(Guid id);
}
