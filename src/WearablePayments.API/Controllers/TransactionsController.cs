namespace WearablePayments.API.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WearablePayments.Core.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _transactions;

    public TransactionsController(ITransactionRepository transactions) => _transactions = transactions;

    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var transactions = await _transactions.GetByUserIdAsync(GetUserId());
        return Ok(transactions.Select(t => new
        {
            t.Id, t.Amount, t.Currency, t.MerchantName, t.MerchantCategory,
            Status = t.Status.ToString(),
            t.AuthorizationCode, t.CreatedAt,
            DeviceName = t.Device.Name,
        }));
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
}
