using Microsoft.AspNetCore.Http;

namespace Product.API.Services;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile file);
    Task DeleteImageAsync(string imageUrl);
}
