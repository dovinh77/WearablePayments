namespace WearablePayments.API.Controllers;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WearablePayments.API.Models;
using WearablePayments.Core.Entities;
using WearablePayments.Core.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IConfiguration _config;

    public AuthController(IUserRepository users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (await _users.GetByEmailAsync(req.Email) is not null)
            return Conflict(new { message = "Email already registered" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = req.Email.ToLowerInvariant(),
            FullName = req.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            CreatedAt = DateTime.UtcNow,
        };

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        return Ok(new AuthResponse(GenerateJwt(user), user.Email, user.FullName));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _users.GetByEmailAsync(req.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(new AuthResponse(GenerateJwt(user), user.Email, user.FullName));
    }

    // Simulated reset token store — use a persistent store (e.g. DB or Redis) in production
    private static readonly Dictionary<string, (string Email, DateTime Expiry)> _resetTokens = new();

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
    {
        var user = await _users.GetByEmailAsync(req.Email);
        if (user is not null)
        {
            var token = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
            _resetTokens[token] = (user.Email, DateTime.UtcNow.AddHours(1));
            // In production: send token via email. Logged here for simulation.
            Console.WriteLine($"[RESET] Token for {user.Email}: {token}");
        }
        // Always return 200 to avoid revealing whether an email exists
        return Ok(new { message = "If that email is registered, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
    {
        if (!_resetTokens.TryGetValue(req.Token, out var entry) || entry.Expiry < DateTime.UtcNow)
            return BadRequest(new { message = "Invalid or expired reset token." });

        var user = await _users.GetByEmailAsync(entry.Email);
        if (user is null)
            return BadRequest(new { message = "Invalid or expired reset token." });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        await _users.SaveChangesAsync();
        _resetTokens.Remove(req.Token);

        return Ok(new { message = "Password reset successfully." });
    }

    private string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "super-secret-dev-key-change-in-prod"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
        };
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "WearablePayments",
            audience: _config["Jwt:Audience"] ?? "WearablePayments",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
