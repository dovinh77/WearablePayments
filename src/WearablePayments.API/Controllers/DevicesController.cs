namespace WearablePayments.API.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WearablePayments.API.Models;
using WearablePayments.Core.Entities;
using WearablePayments.Core.Interfaces;
using WearablePayments.Core.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDeviceRepository _devices;
    private readonly PaymentOrchestrationService _orchestration;

    public DevicesController(IDeviceRepository devices, PaymentOrchestrationService orchestration)
    {
        _devices = devices;
        _orchestration = orchestration;
    }

    [HttpGet]
    public async Task<IActionResult> GetDevices()
    {
        var userId = GetUserId();
        var devices = await _devices.GetByUserIdAsync(userId);
        return Ok(devices.Select(d => new
        {
            d.Id, d.Name, d.SerialNumber, d.BleAddress,
            Status = d.Status.ToString(),
            d.PairedAt,
            HasCredential = d.NfcCredential is not null,
            CredentialStatus = d.NfcCredential?.Status.ToString(),
        }));
    }

    [HttpPost]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest req)
    {
        var device = new Device
        {
            Id = Guid.NewGuid(),
            UserId = GetUserId(),
            Name = req.Name,
            SerialNumber = req.SerialNumber,
            BleAddress = req.BleAddress,
            Status = DeviceStatus.Paired,
            PairedAt = DateTime.UtcNow,
        };
        await _devices.AddAsync(device);
        await _devices.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDevices), new { id = device.Id }, new { device.Id, device.Name });
    }

    [HttpPost("{id}/provision")]
    public async Task<IActionResult> ProvisionDevice(Guid id, [FromBody] ProvisionDeviceRequest req)
    {
        var device = await _devices.GetByIdAsync(id);
        if (device is null || device.UserId != GetUserId())
            return NotFound();

        var credential = await _orchestration.ProvisionDeviceAsync(id, req.CardId);
        return Ok(new
        {
            credential.Id,
            credential.Token,
            credential.Status,
            credential.IssuedAt,
            credential.ExpiresAt,
        });
    }

    [HttpPost("{id}/tap")]
    public async Task<IActionResult> SimulateTap(Guid id, [FromBody] TapPaymentRequest req)
    {
        var device = await _devices.GetByIdAsync(id);
        if (device is null || device.UserId != GetUserId())
            return NotFound();

        var tx = await _orchestration.ProcessTapPaymentAsync(
            id, req.Cryptogram, req.Amount, req.Currency,
            req.MerchantName, req.MerchantCategory);

        return Ok(new
        {
            tx.Id, tx.Amount, tx.Currency, tx.MerchantName,
            Status = tx.Status.ToString(),
            tx.AuthorizationCode,
            tx.CreatedAt,
        });
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
}
