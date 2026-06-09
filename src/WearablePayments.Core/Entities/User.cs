namespace WearablePayments.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ICollection<Device> Devices { get; set; } = new List<Device>();
    public ICollection<PaymentCard> PaymentCards { get; set; } = new List<PaymentCard>();
}
