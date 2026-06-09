namespace WearablePayments.Core.Interfaces;

public interface IDeviceProvisioningService
{
    /// <summary>Push NFC credential to device over simulated BLE channel.</summary>
    Task<bool> ProvisionCredentialAsync(string bleAddress, string token, string cryptogramKey);
    Task<bool> RevokeCredentialAsync(string bleAddress, string token);
}
