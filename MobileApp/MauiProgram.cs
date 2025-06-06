using Microsoft.Extensions.Logging;

namespace JPYCOffline;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services and view models for dependency injection
        builder.Services.AddSingleton<WalletService>();
        builder.Services.AddSingleton<MerchantService>();
        builder.Services.AddSingleton<IAuthenticatorService, AuthenticatorService>();
        builder.Services.AddSingleton<LoginViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}