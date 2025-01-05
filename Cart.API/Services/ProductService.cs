using System.Text.Json;

namespace Cart.API.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public class ProductInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
    }

    public ProductService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["Services:ProductAPI"] ?? "http://localhost:5143/");
    }

    public async Task<ProductInfo?> GetProductAsync(Guid productId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/products/{productId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductInfo>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return product;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> CheckStockAvailabilityAsync(Guid productId, int quantity)
    {
        var product = await GetProductAsync(productId);
        return product?.Stock >= quantity;
    }
}
