using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Devices.Sensors;

namespace MyLocation;

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

        builder.Services.AddSingleton<IGeolocation>(_ => Geolocation.Default);
        builder.Services.AddSingleton<IContacts>(_ => Contacts.Default);
        builder.Services.AddSingleton<ISms>(_ => Sms.Default);
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
