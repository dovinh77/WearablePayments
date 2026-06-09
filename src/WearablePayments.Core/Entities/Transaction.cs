namespace WearablePayments.Core.Entities;

public enum TransactionStatus { Pending, Approved, Declined, Reversed }

public class Transaction
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;
    public Guid NfcCredentialId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "AUD";
    public string MerchantName { get; set; } = string.Empty;
    public string MerchantCategory { get; set; } = string.Empty;
    public TransactionStatus Status { get; set; }
    public string AuthorizationCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
