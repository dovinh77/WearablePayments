namespace WearablePayments.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using WearablePayments.Core.Entities;
using WearablePayments.Core.Interfaces;

public class DeviceRepository : IDeviceRepository
{
    private readonly AppDbContext _db;
    public DeviceRepository(AppDbContext db) => _db = db;

    public Task<Device?> GetByIdAsync(Guid id) =>
        _db.Devices.Include(d => d.NfcCredential).FirstOrDefaultAsync(d => d.Id == id);

    public async Task<IEnumerable<Device>> GetByUserIdAsync(Guid userId) =>
        await _db.Devices.Include(d => d.NfcCredential)
            .Where(d => d.UserId == userId).ToListAsync();

    public async Task AddAsync(Device device) => await _db.Devices.AddAsync(device);
    public async Task AddNfcCredentialAsync(NfcCredential credential) => await _db.NfcCredentials.AddAsync(credential);
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
