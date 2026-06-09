namespace WearablePayments.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WearablePayments.Core.Interfaces;
using WearablePayments.Core.Services;
using WearablePayments.Infrastructure.Data;
using WearablePayments.Infrastructure.Data.Repositories;
using WearablePayments.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=wearable_payments.db"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IPaymentCardRepository, PaymentCardRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        services.AddSingleton<ITokenizationService, SimulatedTokenizationService>();
        services.AddSingleton<IPaymentProcessorService, SimulatedPaymentProcessorService>();
        services.AddScoped<IDeviceProvisioningService, SimulatedDeviceProvisioningService>();
        services.AddScoped<PaymentOrchestrationService>();

        return services;
    }
}
