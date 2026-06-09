namespace WearablePayments.API.Models;

public record RegisterDeviceRequest(string Name, string SerialNumber, string BleAddress);
public record ProvisionDeviceRequest(Guid CardId);
public record TapPaymentRequest(string Cryptogram, decimal Amount, string Currency, string MerchantName, string MerchantCategory);
