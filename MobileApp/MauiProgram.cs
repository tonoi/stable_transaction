using Microsoft.Extensions.Logging;
using ZXing.Net.Maui;
using JPYCOffline.Services;
using CommunityToolkit.Maui;
using Plugin.LocalNotification;
using JPYCOffline.Services.Travel;

namespace JPYCOffline;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .UseMauiCommunityToolkit()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services and view models for dependency injection
        builder.Services.AddSingleton<OfflineWalletService>();
        builder.Services.AddSingleton<MerchantService>();
        builder.Services.AddSingleton<IAuthenticatorService, AuthenticatorService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<LoginViewModel>();

        builder.Services.AddLocalization();
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<ITourService, TourService>();
        builder.Services.AddSingleton<JPYCOffline.Services.Travel.IWalletService, JPYCOffline.Services.Travel.WalletService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}