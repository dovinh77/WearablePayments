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
