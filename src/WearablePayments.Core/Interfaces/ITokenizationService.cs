namespace WearablePayments.Core.Interfaces;

public interface ITokenizationService
{
    Task<(string token, string cryptogramKey)> IssueTokenAsync(string maskedPan, string cardholderName);
    Task<bool> ValidateTokenAsync(string token);
    Task RevokeTokenAsync(string token);
}
