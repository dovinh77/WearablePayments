namespace WearablePayments.Core.Tests.Services;

using Moq;
using WearablePayments.Core.Entities;
using WearablePayments.Core.Interfaces;
using WearablePayments.Core.Services;

public class PaymentOrchestrationServiceTests
{
    private readonly Mock<IDeviceRepository> _devices = new();
    private readonly Mock<IPaymentCardRepository> _cards = new();
    private readonly Mock<ITokenizationService> _tokenization = new();
    private readonly Mock<IDeviceProvisioningService> _provisioning = new();
    private readonly Mock<ITransactionRepository> _transactions = new();
    private readonly Mock<IPaymentProcessorService> _processor = new();

    private PaymentOrchestrationService CreateSut() => new(
        _devices.Object, _cards.Object, _tokenization.Object,
        _provisioning.Object, _transactions.Object, _processor.Object);

    [Fact]
    public async Task ProvisionDevice_HappyPath_ReturnsActiveCredential()
    {
        var deviceId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var device = new Device { Id = deviceId, UserId = Guid.NewGuid(), BleAddress = "AA:BB:CC:DD:EE:FF", Status = DeviceStatus.Paired };
        var card = new PaymentCard { Id = cardId, MaskedPan = "****1234", CardholderName = "Test User" };

        _devices.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync(device);
        _cards.Setup(r => r.GetByIdAsync(cardId)).ReturnsAsync(card);
        _tokenization.Setup(s => s.IssueTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("SIM-TOKEN-abc", "cryptokey123"));
        _provisioning.Setup(s => s.ProvisionCredentialAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await CreateSut().ProvisionDeviceAsync(deviceId, cardId);

        Assert.Equal(CredentialStatus.Active, result.Status);
        Assert.Equal("SIM-TOKEN-abc", result.Token);
        Assert.Equal(DeviceStatus.Provisioned, device.Status);
    }

    [Fact]
    public async Task ProcessTapPayment_Approved_ReturnsApprovedTransaction()
    {
        var deviceId = Guid.NewGuid();
        var credentialId = Guid.NewGuid();
        var credential = new NfcCredential { Id = credentialId, Token = "SIM-TOKEN-xyz", Status = CredentialStatus.Active };
        var device = new Device { Id = deviceId, UserId = Guid.NewGuid(), NfcCredential = credential };

        _devices.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync(device);
        _processor.Setup(s => s.AuthorizeAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(new PaymentResult(true, "AUTH123456", string.Empty));

        var tx = await CreateSut().ProcessTapPaymentAsync(deviceId, "cryptogram", 25.50m, "AUD", "Coffee Shop", "F&B");

        Assert.Equal(TransactionStatus.Approved, tx.Status);
        Assert.Equal("AUTH123456", tx.AuthorizationCode);
        Assert.Equal(25.50m, tx.Amount);
    }
}
