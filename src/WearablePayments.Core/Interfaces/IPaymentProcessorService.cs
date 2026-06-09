namespace WearablePayments.Core.Interfaces;

public record PaymentRequest(string Token, string Cryptogram, decimal Amount, string Currency, string MerchantName);
public record PaymentResult(bool Approved, string AuthorizationCode, string DeclineReason);

public interface IPaymentProcessorService
{
    Task<PaymentResult> AuthorizeAsync(PaymentRequest request);
}
