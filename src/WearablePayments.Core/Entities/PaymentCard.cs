namespace WearablePayments.Core.Entities;

public enum CardNetwork { Visa, Mastercard, Amex }

public class PaymentCard
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string MaskedPan { get; set; } = string.Empty;  // e.g. "****1234"
    public string CardholderName { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public CardNetwork Network { get; set; }
    public bool IsDefault { get; set; }
    public DateTime AddedAt { get; set; }
}
