using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using tremorur.Messages;

namespace tremorur
{
    public static partial class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                   .ConfigureFonts(fonts =>
                   {
                       fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                       fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                   });

            builder.Services.AddTransient<IButtonService, ButtonService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            // Manually create LoggerFactory and Logger
            var loggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddConsole(); // Add console logging or other providers
#if DEBUG
                logging.AddDebug();
#endif
            });
            // Register the logger factory in services, so it's available later
            builder.Services.AddSingleton(loggerFactory);

            var messenger = new Messenger(loggerFactory.CreateLogger<Messenger>());
            builder.Services.AddSingleton<Services.IMessenger>(messenger);

            var mauiApp = builder.Build();
            messenger.Send<AppBuilt>(new(mauiApp.Services));
            return mauiApp;
        }
    }
}
