using Microsoft.AspNetCore.Http;

namespace Product.API.Services;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadDirectory;

    public ImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _uploadDirectory = Path.Combine(_environment.WebRootPath, "images", "products");
        
        // Ensure directory exists
        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    public async Task<string> SaveImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("No file was provided");
        }

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_uploadDirectory, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative URL
        return $"/images/products/{fileName}";
    }

    public Task DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            return Task.CompletedTask;
        }

        var fileName = Path.GetFileName(imageUrl);
        var filePath = Path.Combine(_uploadDirectory, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
