namespace WearablePayments.Core.Interfaces;

using WearablePayments.Core.Entities;

public interface IPaymentCardRepository
{
    Task<PaymentCard?> GetByIdAsync(Guid id);
    Task<IEnumerable<PaymentCard>> GetByUserIdAsync(Guid userId);
    Task AddAsync(PaymentCard card);
    Task RemoveAsync(PaymentCard card);
    Task SaveChangesAsync();
}
