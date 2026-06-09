namespace WearablePayments.Core.Interfaces;

using WearablePayments.Core.Entities;

public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(Guid id);
    Task<IEnumerable<Device>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Device device);
    Task AddNfcCredentialAsync(NfcCredential credential);
    Task SaveChangesAsync();
}
