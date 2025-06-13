using Microsoft.Extensions.Logging;
using ZXing.Net.Maui;
using OfflineGuide.Services;
using CommunityToolkit.Maui;
using Plugin.LocalNotification;
using OfflineGuide.Services.Travel;

namespace OfflineGuide;

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
        builder.Services.AddSingleton<OfflineGuide.Services.Travel.IWalletService, OfflineGuide.Services.Travel.WalletService>();
        builder.Services.AddSingleton<OfflineGuide.Services.Travel.INotificationService, OfflineGuide.Services.Travel.NotificationService>();
        builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}