using Microsoft.Extensions.Logging;
using ZXing.Net.Maui;
using JPYCOffline.Services;

namespace JPYCOffline;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


        builder.Services.AddSingleton<IAuthenticatorService, AuthenticatorService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}