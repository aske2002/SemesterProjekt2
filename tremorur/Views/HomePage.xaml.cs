using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace tremorur.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly ILogger _logger;
        public HomePage(HomeViewModel viewModel, ILogger logger)
        {
            _logger = logger;
            _logger.Log(LogLevel.Information, "Initializing homepage");
            InitializeComponent();
        }
        async void StartClock()
        {
            while (true)
            {
                ClockLabel.Text = DateTime.Now.ToString("HH:mm"); //Opdater label med klokken. Henter den aktuelle tid i 24 timers format
                await Task.Delay(1000); //sørger for at opdateringen sker hvert sekund
            }
        }
        private int level = 1; //vibrationsstart level 1
        void OnOpClicked(object sender, EventArgs e)
        {
            if (level < 7)
            {
                level++;
                UpdateLevelLabel();
            }
        }
        void OnNedClicked(object sender, EventArgs e)
        {
            if(level>1)
            {
                level--;
                UpdateLevelLabel();
            }
        }
        private void UpdateLevelLabel()
        {
            LevelLabel.Text = $"Level:{level}";
            OpBtn.IsEnabled = level < 7;
            NedBtn.IsEnabled = level >1;
        }
        async void OnOkClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Levet Accepteres", $"Du har valgt Level {level}!", "OK");
        }
        async void OnAnnullerClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Annuller", "Udskyd 5 minutter", "Ja", "Nej");
            if (confirm)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
