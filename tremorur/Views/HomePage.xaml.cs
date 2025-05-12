using System.Diagnostics;
using Microsoft.Extensions.Logging;
using shared.Models.Vibrations;

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
        protected async override void OnOKButtonHeld(object? sender, int ms, Action didHandle)
        {
            if(ms>3000) //hvis ok-knappen holdes nede i 3 sekunder 
            {
                didHandle();
                await vibrationsService.StartStopVibration(); //starter vibrationer
                if(BindingContext is HomeViewModel vm)
                {
                    vm.Level = vm.Level == 0 ? 1 : 0;
                }
            }
        }
        protected override async void OnUpButtonClicked(object sender, EventArgs e)
        {
            if (BindingContext is HomeViewModel vm && vm.Level>=1&&vm.Level<7)
                vm.Level++;

            await vibrationsService.NavigateLevelUp();
        }
        protected override async void OnDownButtonClicked(object sender, EventArgs e)
        {
            if (BindingContext is HomeViewModel vm && vm.Level > 1)
                vm.Level--;

            await vibrationsService.NavigateLevelDown();
        }
        protected async override void OnUpButtonHeld(object? sender, int ms, Action didHandle)
        {
            if (ms > 3000) //hvis up-knappen holdes nede i 3 sekunder 
            {
                didHandle();
                await navigationService.GoToAsync("//setAlarm"); //går til SetAlarmPage
            }
        }
    }
}
