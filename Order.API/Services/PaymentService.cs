using System.Text.Json;

namespace Order.API.Services;

public class PaymentService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public class PaymentInfo
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public PaymentService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["Services:PaymentAPI"] ?? "http://localhost:5005/");
    }

    public async Task<PaymentInfo?> InitiatePaymentAsync(Guid orderId, decimal amount, string paymentMethod)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/payments/process", new
            {
                OrderId = orderId,
                Amount = amount,
                PaymentMethod = paymentMethod
            });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var paymentInfo = JsonSerializer.Deserialize<PaymentInfo>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return paymentInfo;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string?> GetPaymentStatusAsync(string paymentIntentId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/payments/{paymentIntentId}/status");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var paymentInfo = JsonSerializer.Deserialize<PaymentInfo>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return paymentInfo?.Status;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
