using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Authentication.API.Models;
using Authentication.API.Models.DTOs;

namespace Authentication.API.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly MongoDbService _mongoDb;
    private readonly EmailService _emailService;

    public AuthService(IConfiguration configuration, MongoDbService mongoDb, EmailService emailService)
    {
        _configuration = configuration;
        _mongoDb = mongoDb;
        _emailService = emailService;
    }

    public ClaimsPrincipal? GetTokenClaims(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> RevokeToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var revokedToken = new RevokedToken
            {
                Token = token,
                ExpiresAt = jwtToken.ValidTo
            };

            await _mongoDb.AddRevokedToken(revokedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsTokenRevoked(string token)
    {
        return await _mongoDb.IsTokenRevoked(token);
    }

    public async Task<(User? User, string? Token)> Register(RegisterDTO model)
    {
        if (await _mongoDb.GetUserByEmail(model.Email) != null)
            return (null, null);

        var user = new User
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
        };

        await _mongoDb.CreateUser(user);

        var token = await GenerateJwtToken(user);
        return (user, token);
    }

    public async Task<(User? User, string? Token)> Login(LoginDTO model)
    {
        var user = await _mongoDb.GetUserByEmail(model.Email);
        if (user == null) return (null, null);

        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return (null, null);

        var token = await GenerateJwtToken(user);
        return (user, token);
    }

    public async Task<User?> GetUserById(string id)
    {
        return await _mongoDb.GetUserById(id);
    }

    public async Task<User?> UpdateProfile(string userId, UpdateProfileDTO model)
    {
        var user = await _mongoDb.GetUserById(userId);
        if (user == null) return null;

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.UpdatedAt = DateTime.UtcNow;

        await _mongoDb.UpdateUser(user.Id, user);
        return user;
    }

    private string GenerateRandomCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public async Task<bool> GeneratePasswordResetToken(string email)
    {
        var user = await _mongoDb.GetUserByEmail(email);
        if (user == null) return false;

        string resetCode = GenerateRandomCode();

        user.ResetPasswordToken = resetCode;
        user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(2);
        user.UpdatedAt = DateTime.UtcNow;

        await _mongoDb.UpdateUser(user.Id, user);

        await _emailService.SendPasswordResetCode(email, resetCode);

        return true;
    }

    public async Task<(bool Success, string Message)> ResetPassword(string email, string code, string newPassword)
    {
        var user = await _mongoDb.GetUserByEmail(email);

        if (user == null)
        {
            return (false, "Code incorrect");
        }

        if (user.ResetPasswordTokenExpiry < DateTime.UtcNow)
        {
            return (false, "Code expiré");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _mongoDb.UpdateUser(user.Id, user);
        return (true, "Mot de passe réinitialisé avec succès");
    }

    public async Task<bool> ChangePassword(string userId, string currentPassword, string newPassword)
    {
        var user = await _mongoDb.GetUserById(userId);
        if (user == null) return false;

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _mongoDb.UpdateUser(user.Id, user);
        return true;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _mongoDb.GetAllUsers();
    }

    public async Task<bool> UpdateUserRole(string userId, string role)
    {
        var user = await _mongoDb.GetUserById(userId);
        if (user == null) return false;

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;

        await _mongoDb.UpdateUser(user.Id, user);
        return true;
    }
}