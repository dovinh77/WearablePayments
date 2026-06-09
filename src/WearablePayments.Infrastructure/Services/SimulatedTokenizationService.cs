namespace WearablePayments.Infrastructure.Services;

using System.Security.Cryptography;
using WearablePayments.Core.Interfaces;

public class SimulatedTokenizationService : ITokenizationService
{
    private static readonly HashSet<string> _activeTokens = new();

    public Task<(string token, string cryptogramKey)> IssueTokenAsync(string maskedPan, string cardholderName)
    {
        var token = $"SIM-TOKEN-{RandomHex(16)}";
        var key = RandomHex(32);
        _activeTokens.Add(token);
        return Task.FromResult((token, key));
    }

    public Task<bool> ValidateTokenAsync(string token) =>
        Task.FromResult(_activeTokens.Contains(token));

    public Task RevokeTokenAsync(string token)
    {
        _activeTokens.Remove(token);
        return Task.CompletedTask;
    }

    private static string RandomHex(int bytes) =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(bytes)).ToLower();
}
