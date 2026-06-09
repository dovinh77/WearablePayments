namespace WearablePayments.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using WearablePayments.Core.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<PaymentCard> PaymentCards => Set<PaymentCard>();
    public DbSet<NfcCredential> NfcCredentials => Set<NfcCredential>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e => {
            e.HasIndex(u => u.Email).IsUnique();
            e.HasMany(u => u.Devices).WithOne(d => d.User).HasForeignKey(d => d.UserId);
            e.HasMany(u => u.PaymentCards).WithOne(c => c.User).HasForeignKey(c => c.UserId);
        });

        modelBuilder.Entity<Device>(e => {
            e.HasIndex(d => d.SerialNumber).IsUnique();
            e.HasOne(d => d.NfcCredential).WithOne(c => c.Device).HasForeignKey<NfcCredential>(c => c.DeviceId);
        });

        modelBuilder.Entity<Transaction>(e => {
            e.Property(t => t.Amount).HasPrecision(18, 4);
        });
    }
}
