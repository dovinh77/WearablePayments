namespace WearablePayments.Core.Entities;

public enum CredentialStatus { Active, Suspended, Expired }

public class NfcCredential
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;
    public Guid PaymentCardId { get; set; }
    public string Token { get; set; } = string.Empty;       // simulated payment token
    public string CryptogramKey { get; set; } = string.Empty; // simulated key for tap
    public CredentialStatus Status { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
