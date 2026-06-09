namespace WearablePayments.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using WearablePayments.Core.Entities;
using WearablePayments.Core.Interfaces;

public class PaymentCardRepository : IPaymentCardRepository
{
    private readonly AppDbContext _db;
    public PaymentCardRepository(AppDbContext db) => _db = db;

    public Task<PaymentCard?> GetByIdAsync(Guid id) => _db.PaymentCards.FindAsync(id).AsTask();

    public async Task<IEnumerable<PaymentCard>> GetByUserIdAsync(Guid userId) =>
        await _db.PaymentCards.Where(c => c.UserId == userId).ToListAsync();

    public async Task AddAsync(PaymentCard card) => await _db.PaymentCards.AddAsync(card);

    public Task RemoveAsync(PaymentCard card)
    {
        _db.PaymentCards.Remove(card);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
