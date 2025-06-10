using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace JapanTravelApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        // Add localization services
        builder.Services.AddLocalization();

        // Add services for the app
        builder.Services.AddSingleton<Services.IAuthenticationService, Services.AuthenticationService>();
        builder.Services.AddSingleton<Services.ITourService, Services.TourService>();
        builder.Services.AddSingleton<Services.IWalletService, Services.WalletService>();
        builder.Services.AddSingleton<Services.INotificationService, Services.NotificationService>();
        builder.Services.AddSingleton<Services.ILocalizationService, Services.LocalizationService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
