using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WearablePayments.App.Services;
using WearablePayments.App.ViewModels;
using WearablePayments.App.Views;

namespace WearablePayments.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddHttpClient<ApiService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5000/");
        });
        builder.Services.AddSingleton<BleService>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
