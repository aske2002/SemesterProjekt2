using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using shared.Models.Vibrations;
using shared.Models.Vibrations.Patterns;
using tremorur.Development.HotReload;
using tremorur.Messages;

using Windows.ApplicationModel.DataTransfer.DragDrop.Core;
using Windows.Graphics.Display;

namespace tremorur.Views
{
    public partial class HomePage : ContentPageWithButtons
    {
        private readonly ILogger _logger;
        private readonly IButtonService _buttonService;
        private readonly Services.IMessenger _messenger;
        private readonly INavigationService navigationService;
        private readonly VibrationsService vibrationsService;
        public HomePage(HomeViewModel viewModel, ILogger<HomePage> logger, IButtonService buttonService, Services.IMessenger messenger, INavigationService navigationService, VibrationsService vibrationsService) : base(buttonService)
        {
            _logger = logger;
            _logger.Log(LogLevel.Information, "Initializing homepage");
            InitializeComponent();
            BindingContext = viewModel;
            this.navigationService = navigationService;
            StartClock();
            this.vibrationsService = vibrationsService;
        }
        async void StartClock()
        {
            while (true)
            {
                DateTime now = DateTime.Now;
                TimeSpan currentTime = now.TimeOfDay; // Henter tidspunktet som TimeSpan
                string date = now.ToString("ddd dd. MMM"); // Formatterer dato som ugedag/dato/måned

                ClockLabel.Text = $"{currentTime.Hours:D2}:{currentTime.Minutes:D2}"; // Viser tid
                DateLabel.Text = date; // Opdaterer datoen i en separat label

                await Task.Delay(1000); // Opdater hvert sekund
            }
        }

        //protected async override void OnUpButtonHeld(object? sender, int ms)
        //{
        //    var id = Guid.NewGuid();
        //    Debug.WriteLine($"Id: {id}");
        //    var constant = new VibrationPatternConstant(0.5, 1000);
        //    var expression = VibrationSettings.CreateSinePatternSettings(1.0).Pattern;
        //    var dynamic = new VibrationPatternDynamic(new List<VibrationPatternDynamic.VibrationPatternSegment>()
        //        {
        //            new VibrationPatternDynamic.VibrationPatternSegment(100, 0.5),
        //            new VibrationPatternDynamic.VibrationPatternSegment(1000, 0.25),
        //            new VibrationPatternDynamic.VibrationPatternSegment(10000, 1.0)
        //        }, 1000);
        //    var mixed = new VibrationPatternMixed(new List<VibrationPatternMixed.VibrationPatternSegment>()
        //        {
        //            new VibrationPatternMixed.VibrationPatternSegment(constant, 1000),
        //            new VibrationPatternMixed.VibrationPatternSegment(expression, 1000),
        //            new VibrationPatternMixed.VibrationPatternSegment(dynamic, 1000)
        //        }, 1000);
        //    VibrationSettings settings = new VibrationSettings()
        //    {
        //        Id = id,
        //        Pattern = mixed
        //    };
        //    var binary = settings.ToBytes();
        //    var parsed = await VibrationSettings.FromBytes(binary);
        //    Debug.WriteLine($"Parsed: {parsed}");
        //}

        protected async override void OnOKButtonHeld(object? sender, int ms, Action didHandle)
        {
            if(ms>3000) //hvis ok-knappen holdes nede i 3 sekunder 
            {
                await vibrationsService.StartStopVibration(); //starter vibrationer
                await navigationService.GoToAsync("//setVibration"); //går til SetVibrationsPage
            }
        }
        protected async override void OnUpButtonHeld(object? sender, int ms, Action didHandle)
        {
            if (ms > 3000) //hvis up-knappen holdes nede i 3 sekunder 
            {
                await navigationService.GoToAsync("//setAlarm"); //går til SetAlarmPage
            }
        }
    }
}
