namespace Authentication.API.Models;

public class TokenValidationMessage
{
    public string Token { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? UserId { get; set; }
    public string? Role { get; set; }
    public string? Message { get; set; }
}