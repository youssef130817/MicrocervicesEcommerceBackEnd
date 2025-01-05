namespace Order.API.Models;

public class TokenValidationResponse
{
    public string RequestId { get; set; } = string.Empty;
    public TokenValidationMessage Message { get; set; } = new();
}