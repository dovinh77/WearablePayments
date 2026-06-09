namespace WearablePayments.Core.Services;

using WearablePayments.Core.Entities;
using WearablePayments.Core.Interfaces;

public class PaymentOrchestrationService
{
    private readonly IDeviceRepository _devices;
    private readonly IPaymentCardRepository _cards;
    private readonly ITokenizationService _tokenization;
    private readonly IDeviceProvisioningService _provisioning;
    private readonly ITransactionRepository _transactions;
    private readonly IPaymentProcessorService _processor;

    public PaymentOrchestrationService(
        IDeviceRepository devices,
        IPaymentCardRepository cards,
        ITokenizationService tokenization,
        IDeviceProvisioningService provisioning,
        ITransactionRepository transactions,
        IPaymentProcessorService processor)
    {
        _devices = devices;
        _cards = cards;
        _tokenization = tokenization;
        _provisioning = provisioning;
        _transactions = transactions;
        _processor = processor;
    }

    public async Task<NfcCredential> ProvisionDeviceAsync(Guid deviceId, Guid cardId)
    {
        var device = await _devices.GetByIdAsync(deviceId)
            ?? throw new InvalidOperationException("Device not found");
        var card = await _cards.GetByIdAsync(cardId)
            ?? throw new InvalidOperationException("Card not found");

        var (token, cryptogramKey) = await _tokenization.IssueTokenAsync(card.MaskedPan, card.CardholderName);

        var pushed = await _provisioning.ProvisionCredentialAsync(device.BleAddress, token, cryptogramKey);
        if (!pushed)
            throw new InvalidOperationException("Failed to provision credential to device");

        var credential = new NfcCredential
        {
            Id = Guid.NewGuid(),
            DeviceId = deviceId,
            PaymentCardId = cardId,
            Token = token,
            CryptogramKey = cryptogramKey,
            Status = CredentialStatus.Active,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddYears(3),
        };

        await _devices.AddNfcCredentialAsync(credential);
        device.Status = DeviceStatus.Provisioned;
        await _devices.SaveChangesAsync();

        return credential;
    }

    public async Task<Transaction> ProcessTapPaymentAsync(
        Guid deviceId, string cryptogram, decimal amount, string currency,
        string merchantName, string merchantCategory)
    {
        var device = await _devices.GetByIdAsync(deviceId)
            ?? throw new InvalidOperationException("Device not found");

        var credential = device.NfcCredential
            ?? throw new InvalidOperationException("Device has no provisioned credential");

        var request = new PaymentRequest(credential.Token, cryptogram, amount, currency, merchantName);
        var result = await _processor.AuthorizeAsync(request);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            DeviceId = deviceId,
            NfcCredentialId = credential.Id,
            Amount = amount,
            Currency = currency,
            MerchantName = merchantName,
            MerchantCategory = merchantCategory,
            Status = result.Approved ? TransactionStatus.Approved : TransactionStatus.Declined,
            AuthorizationCode = result.AuthorizationCode,
            CreatedAt = DateTime.UtcNow,
        };

        await _transactions.AddAsync(transaction);
        await _transactions.SaveChangesAsync();

        return transaction;
    }
}
