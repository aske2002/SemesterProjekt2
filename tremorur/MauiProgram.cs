using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using tremorur.Messages;

namespace tremorur
{
    public static partial class MauiProgram
    {
        public static MauiApp CreateMauiApp() //sker inden programmet starter op
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiCommunityToolkit();
            builder.UseMauiApp<App>()
                   .ConfigureFonts(fonts =>
                   {
                       fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                       fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                   });
            builder.Services.AddSingleton<IBluetoothService,BluetoothService>();
            builder.Services.AddSingleton<IButtonService, ButtonService>();
            builder.Services.AddSingleton<VibrationsService>();
        
            
            builder.Services.AddTransient<AlarmService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<INavigationService, NavigationService>(); //bruges til at navigere rundt med. Har en goto, hvordan vi navigerer rundt mellem mapperne. 

            builder.Services.AddSingleton<HomeViewModel>(); 
            builder.Services.AddSingleton<HomePage>();

            builder.Services.AddTransient<MedicationAlarmViewModel>();
            builder.Services.AddTransient<MedicationAlarmPage>();

            builder.Services.AddTransient<SetAlarmViewModel>();
            builder.Services.AddTransient<SetAlarmPage>();
            

            builder.Services.AddTransient<BluetoothConnectPage>();
            builder.Services.AddTransient<BluetoothConnectViewModel>();

            builder.Services.AddTransient<BluetoothDevPage>();
            builder.Services.AddTransient<BluetoothDevViewModel>();

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
            messenger.SendMessage<AppBuilt>(new(mauiApp.Services));
            return mauiApp; //n�r dette er k�rt igennem, starter vinduet op 
        }
    }
}
