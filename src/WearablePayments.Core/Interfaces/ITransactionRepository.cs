namespace WearablePayments.Core.Interfaces;

using WearablePayments.Core.Entities;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetByDeviceIdAsync(Guid deviceId);
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Transaction transaction);
    Task SaveChangesAsync();
}
