namespace WearablePayments.API.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WearablePayments.Core.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserRepository _users;

    public ProfileController(IUserRepository users) => _users = users;

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _users.GetByIdAsync(GetUserId());
        if (user is null) return NotFound();
        return Ok(new { user.FullName, user.Email });
    }

    [HttpPost("update-name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateNameRequest req)
    {
        var user = await _users.GetByIdAsync(GetUserId());
        if (user is null) return NotFound();

        user.FullName = req.FullName;
        await _users.SaveChangesAsync();
        return Ok(new { user.FullName });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
    {
        var user = await _users.GetByIdAsync(GetUserId());
        if (user is null) return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, user.PasswordHash))
            return BadRequest(new { message = "Current password is incorrect." });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        await _users.SaveChangesAsync();
        return Ok(new { message = "Password changed successfully." });
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public record UpdateNameRequest(string FullName);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
