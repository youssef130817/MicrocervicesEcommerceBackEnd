using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Authentication.API.Models;
using Authentication.API.Models.DTOs;
using Authentication.API.Services;

namespace Authentication.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO model)
    {
        var (user, token) = await _authService.Register(model);
        if (user == null)
            return BadRequest(new { message = "L'email existe déjà" });

        return Ok(new { token, user });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        var (user, token) = await _authService.Login(model);
        if (user == null)
            return BadRequest(new { message = "Identifiants invalides" });

        return Ok(new { token, user });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return BadRequest(new { message = "Token non fourni" });

        var token = authHeader.Substring("Bearer ".Length).Trim();
        var success = await _authService.RevokeToken(token);

        if (!success)
            return BadRequest(new { message = "Échec de la déconnexion" });

        return Ok(new { message = "Déconnexion réussie" });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var user = await _authService.GetUserById(userId);
        if (user == null) return NotFound();

        return Ok(user);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDTO model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var user = await _authService.UpdateProfile(userId, model);
        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ResetPasswordDTO model)
    {
        var result = await _authService.GeneratePasswordResetToken(model.Email);
        if (!result)
            return BadRequest(new { message = "Email non trouvé" });

        return Ok(new { message = "Un code de réinitialisation a été envoyé à votre email" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ChangePasswordDTO model)
    {
        var (success, message) = await _authService.ResetPassword(model.Email, model.Code, model.NewPassword);
        if (!success)
            return BadRequest(new { message });

        return Ok(new { message });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangeCurrentPasswordDTO model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var result = await _authService.ChangePassword(userId, model.CurrentPassword, model.NewPassword);
        if (!result)
            return BadRequest(new { message = "Mot de passe actuel incorrect" });

        return Ok(new { message = "Mot de passe changé avec succès" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _authService.GetAllUsers();
        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> UpdateUserRole(string userId, [FromBody] UpdateRoleDTO model)
    {
        if (!new[] { "Admin", "User" }.Contains(model.Role))
            return BadRequest(new { message = "Rôle invalide" });

        var success = await _authService.UpdateUserRole(userId, model.Role);
        if (!success)
            return NotFound(new { message = "Utilisateur non trouvé" });

        return Ok(new { message = "Rôle mis à jour avec succès" });
    }
}