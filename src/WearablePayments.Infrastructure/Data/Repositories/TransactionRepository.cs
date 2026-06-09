namespace WearablePayments.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using WearablePayments.Core.Entities;
using WearablePayments.Core.Interfaces;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;
    public TransactionRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Transaction>> GetByDeviceIdAsync(Guid deviceId) =>
        await _db.Transactions.Where(t => t.DeviceId == deviceId)
            .OrderByDescending(t => t.CreatedAt).ToListAsync();

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId) =>
        await _db.Transactions
            .Include(t => t.Device)
            .Where(t => t.Device.UserId == userId)
            .OrderByDescending(t => t.CreatedAt).ToListAsync();

    public async Task AddAsync(Transaction transaction) => await _db.Transactions.AddAsync(transaction);
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
