namespace WearablePayments.Core.Entities;

public enum DeviceStatus { Unpaired, Paired, Provisioned, Suspended }

public class Device
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string BleAddress { get; set; } = string.Empty;
    public DeviceStatus Status { get; set; }
    public DateTime PairedAt { get; set; }
    public NfcCredential? NfcCredential { get; set; }
}
