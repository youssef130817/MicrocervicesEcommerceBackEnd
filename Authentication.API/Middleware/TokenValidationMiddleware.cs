using Authentication.API.Services;
using Authentication.API.Models;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.API.Middleware;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public TokenValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            using var scope = _serviceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
            var kafkaService = scope.ServiceProvider.GetRequiredService<KafkaProducerService>();

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var isRevoked = await authService.IsTokenRevoked(token);

            var validationMessage = new TokenValidationMessage
            {
                Token = token,
                IsValid = !isRevoked
            };

            if (isRevoked)
            {
                validationMessage.Message = "Token révoqué";
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "Token révoqué" });
            }
            else
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
                validationMessage.UserId = userId;
                validationMessage.Role = role;
                validationMessage.Message = "Token valide";
            }

            var requestId = Guid.NewGuid().ToString();
            await kafkaService.PublishValidationResponse(requestId, validationMessage);

            if (isRevoked)
            {
                return;
            }
        }

        await _next(context);
    }
}