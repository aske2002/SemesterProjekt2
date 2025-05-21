using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Logging;
using shared.Models.Vibrations;
using tremorur.Messages;

namespace tremorur.Views
{
    public partial class HomePage : ContentPageWithButtons
    {
        private readonly ILogger _logger;
        private readonly INavigationService navigationService;
        private readonly VibrationsService vibrationsService;
        public HomePage(HomeViewModel viewModel, ILogger<HomePage> logger, IButtonService buttonService, INavigationService navigationService, VibrationsService vibrationsService) : base(buttonService)
        {
            _logger = logger;
            _logger.Log(LogLevel.Information, "Initializing homepage");
            InitializeComponent();
            BindingContext = viewModel;
            this.navigationService = navigationService;
            StartClock();
            this.vibrationsService = vibrationsService;
            this.vibrationsService.VibrationLevelChanged += (sender, level) =>
            {
                if (BindingContext is HomeViewModel vm)
                {
                    vm.Level = level + 1;
                }
            };
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
            if (ms > 3000) //hvis ok-knappen holdes nede i 3 sekunder 
            {
                didHandle();
                await vibrationsService.StartStopVibration(); //starter vibrationer
            }
        }
        protected override async void OnUpButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                await vibrationsService.NavigateLevelUp();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Error navigating up: " + ex.Message);
                await Shell.Current.DisplayAlert("Error", "Error navigating up: " + ex.Message, "OK");
            }
        }
        protected override async void OnDownButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                await vibrationsService.NavigateLevelDown();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Error navigating down: " + ex.Message);
                await Shell.Current.DisplayAlert("Error", "Error navigating down: " + ex.Message, "OK");
            }
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
