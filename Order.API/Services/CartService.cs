using System.Text.Json;

namespace Order.API.Services;

public class CartService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public class CartInfo
    {
        public Guid Id { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }

    public class CartItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }

    public CartService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["Services:CartAPI"] ?? "http://localhost:5004/");
    }

    public async Task<CartInfo?> GetCartAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/cart");
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var cart = JsonSerializer.Deserialize<CartInfo>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return cart;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> ClearCartAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync("api/cart/clear", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
