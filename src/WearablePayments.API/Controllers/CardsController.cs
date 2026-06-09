namespace WearablePayments.API.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WearablePayments.API.Models;
using WearablePayments.Core.Entities;
using WearablePayments.Core.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CardsController : ControllerBase
{
    private readonly IPaymentCardRepository _cards;

    public CardsController(IPaymentCardRepository cards) => _cards = cards;

    [HttpGet]
    public async Task<IActionResult> GetCards()
    {
        var cards = await _cards.GetByUserIdAsync(GetUserId());
        return Ok(cards.Select(c => new
        {
            c.Id, c.MaskedPan, c.CardholderName,
            c.ExpiryMonth, c.ExpiryYear,
            Network = c.Network.ToString(),
            c.IsDefault, c.AddedAt,
        }));
    }

    [HttpPost]
    public async Task<IActionResult> AddCard([FromBody] AddCardRequest req)
    {
        if (!Enum.TryParse<CardNetwork>(req.Network, ignoreCase: true, out var network))
            return BadRequest(new { message = "Invalid card network. Use Visa, Mastercard, or Amex." });

        // Mask PAN — keep only last 4 digits
        var masked = $"****{req.Pan[^4..]}";

        var existingCards = await _cards.GetByUserIdAsync(GetUserId());
        var isDefault = !existingCards.Any();

        var card = new PaymentCard
        {
            Id = Guid.NewGuid(),
            UserId = GetUserId(),
            MaskedPan = masked,
            CardholderName = req.CardholderName,
            ExpiryMonth = req.ExpiryMonth,
            ExpiryYear = req.ExpiryYear,
            Network = network,
            IsDefault = isDefault,
            AddedAt = DateTime.UtcNow,
        };

        await _cards.AddAsync(card);
        await _cards.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCards), new { id = card.Id }, new { card.Id, card.MaskedPan });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveCard(Guid id)
    {
        var card = await _cards.GetByIdAsync(id);
        if (card is null || card.UserId != GetUserId())
            return NotFound();

        await _cards.RemoveAsync(card);
        await _cards.SaveChangesAsync();
        return NoContent();
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
}
