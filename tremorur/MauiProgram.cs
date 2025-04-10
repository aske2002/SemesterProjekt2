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
            builder.UseMauiApp<App>() //man kan ikke lave maui app uden builder 
                   .ConfigureFonts(fonts =>
                   {
                       fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                       fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                   });

            builder.Services.AddTransient<AlarmService>();
            builder.Services.AddTransient<IButtonService, ButtonService>();
            //alt dette er dependency injection - man bruger serviceCollection
            builder.Services.AddTransient<IButtonService, ButtonService>();//bruger den instans af dialogservice gennem det hele - singleton gør at den er sådan hele vejen igennem 
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<INavigationService, NavigationService>(); //bruges til at navigere rundt med. Har en goto, hvordan vi navigerer rundt mellem mapperne. 

            builder.Services.AddSingleton<HomeViewModel>(); 
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<MedicationAlarmViewModel>();
            builder.Services.AddSingleton<SearchPage>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddTransient<SetAlarmViewModel>();
            builder.Services.AddTransient<SetAlarmPage>();
            builder.Services.AddTransient<NewEventViewModel>();
            builder.Services.AddTransient<NewEventPage>();

            // Manually create LoggerFactory and Logger - så man kan få noget ud i consollen - hjælp 
            var loggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddConsole(); // Add console logging or other providers
#if DEBUG
                logging.AddDebug();
#endif
            });
            // Register the logger factory in services, so it's available later
            builder.Services.AddSingleton<ILoggerFactory>(loggerFactory);
            var messenger = new Messenger(loggerFactory.CreateLogger<Messenger>());
            builder.Services.AddSingleton<Services.IMessenger>(messenger);
            var messenger = new DefaultMessenger(loggerFactory.CreateLogger<DefaultMessenger>());
            builder.Services.AddSingleton<Services.IMessenger>(messenger); //singelton - statisk object - har ikke nogen instans metode. 

            var mauiApp = builder.Build();
            messenger.SendBegivenhed<AppBuilt>(new(mauiApp.Services));
            return mauiApp;
            messenger.Send<AppBuilt>(new(mauiApp.Services));
            return mauiApp; //når dette er kørt igennem, starter vinduet op 
        }
    }
}
