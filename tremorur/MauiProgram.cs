using System.Globalization;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using tremorur.Views;
using shared.Models;
using tremorur.Messages;

namespace tremorur
{
    public static partial class MauiProgram
    {
        public static MauiApp CreateMauiApp() //sker inden programmet starter op
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            var builder = MauiApp.CreateBuilder();
            builder.UseMauiCommunityToolkit();
            builder.UseMauiApp<App>()
                   .ConfigureFonts(fonts =>
                   {
                       fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                       fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                   });
            builder.Services.AddSingleton<IBluetoothService, BluetoothService>();
            builder.Services.AddSingleton<IBluetoothStateManager, BluetoothStateManager>();
            builder.Services.AddSingleton<IButtonService, ButtonService>();
            builder.Services.AddSingleton<VibrationsService>();

            builder.Services.AddTransient<AlarmService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>(); //bruges til at navigere rundt med. Har en goto, hvordan vi navigerer rundt mellem mapperne. 

            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<HomePage>();

            builder.Services.AddSingleton<MedicationAlarmPage>();

            builder.Services.AddSingleton<SetAlarmPage>();

            var loggerProvider = new CustomLoggingProvider();

            // Register the logger factory in services, so it's available later
            builder.Logging.ClearProviders(); // Clear default providers
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddProvider(loggerProvider);
            });

            var messenger = new Messenger(CustomLoggingProvider.CreateLogger<Services.IMessenger>()); // Create a new instance of the messenger with the logger
            builder.Services.AddSingleton<Services.IMessenger>(messenger);

            builder.Services.AddTransient<AlarmListPage>();
            builder.Services.AddTransient<AlarmListViewModel>();



            var mauiApp = builder.Build();
            messenger.SendMessage<AppBuilt>(new(mauiApp.Services));
            return mauiApp; //n�r dette er k�rt igennem, starter vinduet op 
        }
    }
}
