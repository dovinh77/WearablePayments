namespace WearablePayments.Core.Interfaces;

using WearablePayments.Core.Entities;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
