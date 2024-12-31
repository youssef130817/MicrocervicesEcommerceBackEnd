using Product.API.Models;

namespace Product.API.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(Guid id);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Guid id);
}
