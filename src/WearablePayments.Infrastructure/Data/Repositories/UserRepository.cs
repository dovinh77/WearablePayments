namespace WearablePayments.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using WearablePayments.Core.Entities;
using WearablePayments.Core.Interfaces;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id) => _db.Users.FindAsync(id).AsTask();
    public Task<User?> GetByEmailAsync(string email) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());
    public async Task AddAsync(User user) => await _db.Users.AddAsync(user);
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
