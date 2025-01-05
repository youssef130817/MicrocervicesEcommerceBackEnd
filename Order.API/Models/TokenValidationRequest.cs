namespace Order.API.Models;

public class TokenValidationRequest
{
    public string RequestId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
}