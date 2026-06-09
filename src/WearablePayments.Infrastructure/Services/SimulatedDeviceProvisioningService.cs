namespace WearablePayments.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using WearablePayments.Core.Interfaces;

public class SimulatedDeviceProvisioningService : IDeviceProvisioningService
{
    private readonly ILogger<SimulatedDeviceProvisioningService> _logger;

    public SimulatedDeviceProvisioningService(ILogger<SimulatedDeviceProvisioningService> logger)
        => _logger = logger;

    public Task<bool> ProvisionCredentialAsync(string bleAddress, string token, string cryptogramKey)
    {
        _logger.LogInformation("Simulated BLE push to {BleAddress}: token provisioned", bleAddress);
        return Task.FromResult(true);
    }

    public Task<bool> RevokeCredentialAsync(string bleAddress, string token)
    {
        _logger.LogInformation("Simulated BLE revoke at {BleAddress}", bleAddress);
        return Task.FromResult(true);
    }
}
