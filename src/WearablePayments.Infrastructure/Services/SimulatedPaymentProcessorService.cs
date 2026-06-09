namespace WearablePayments.Infrastructure.Services;

using System.Security.Cryptography;
using WearablePayments.Core.Interfaces;

public class SimulatedPaymentProcessorService : IPaymentProcessorService
{
    // Simulate 95% approval rate; decline amounts over 10000
    public Task<PaymentResult> AuthorizeAsync(PaymentRequest request)
    {
        if (request.Amount > 10_000m)
            return Task.FromResult(new PaymentResult(false, string.Empty, "Amount exceeds limit"));

        var roll = RandomNumberGenerator.GetInt32(100);
        if (roll < 5)
            return Task.FromResult(new PaymentResult(false, string.Empty, "Simulated decline"));

        var authCode = $"AUTH{RandomNumberGenerator.GetInt32(100000, 999999)}";
        return Task.FromResult(new PaymentResult(true, authCode, string.Empty));
    }
}
