using Microsoft.Extensions.Logging;
using tremorur.Development.HotReload;
using tremorur.Messages;

namespace tremorur.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly ILogger _logger;
        private readonly IButtonService _buttonService;
        private readonly Services.IMessenger _messenger;
        public HomePage(HomeViewModel viewModel, ILogger<HomePage> logger, IButtonService buttonService, Services.IMessenger messenger)
        {
            _logger = logger;
            _logger.Log(LogLevel.Information, "Initializing homepage");
            InitializeComponent();
            BindingContext = viewModel;
            StartClock();
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

        //private int level = 1; //vibrationsstart level 1
        //void OnOpClicked(object sender, EventArgs e)
        //{
        //    if (level < 7)
        //    {
        //        level++;
        //        UpdateLevelLabel();
        //    }
        //}
        //void OnNedClicked(object sender, EventArgs e)
        //{
        //    if (level > 1)
        //    {
        //        level--;
        //        UpdateLevelLabel();
        //    }
        //}

        //private void UpdateLevelLabel()
        //{
        //    LevelLabel.Text = $"Level:{level}";
        //    OpBtn.IsEnabled = level < 7;
        //    NedBtn.IsEnabled = level > 1;
        //}
        //async void OnAnnullerClicked(object sender, EventArgs e)
        //{
        //    bool confirm = await DisplayAlert("Annuller", "Udskyd 5 minutter", "Ja", "Nej");
        //    if (confirm)
        //    {
        //        await Navigation.PopAsync();
        //    }
        //}
    }
}
