using Microsoft.Extensions.Logging;
using tremorur.Development.HotReload;
using tremorur.Messages;

namespace tremorur.Views
{
    public partial class HomePage : ContentPageWithButtons
    {
        private readonly ILogger _logger;
        private readonly IButtonService _buttonService;
        private readonly Services.IMessenger _messenger;
        private readonly INavigationService navigationService;
        public HomePage(HomeViewModel viewModel, ILogger<HomePage> logger, IButtonService buttonService, Services.IMessenger messenger, INavigationService navigationService) : base(buttonService)
        {
            _logger = logger;
            _logger.Log(LogLevel.Information, "Initializing homepage");
            InitializeComponent();
            BindingContext = viewModel;
            this.navigationService = navigationService;
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

        private int level = 0; //vibrationsstart level 0, hvor 0 er off
        protected override void OnUpButtonClicked(object? sender, EventArgs e)
        {
            if (level < 7)
            {
                level++;
                //UpdateLevelLabel();
            }
        }
        protected override void OnDownButtonClicked(object? sender, EventArgs e)
        {
            if (level > 1)
            {
                level--;
                //UpdateLevelLabel();
            }
        }

        //private void UpdateLevelLabel()
        //{
        //    LevelLabel.Text = $"Level:{level}";
        //    OnUpButtonClicked.IsEnabled = level < 7;
        //    OnDownButtonClicked.IsEnabled = level > 1;
        //}
        protected override async void OnCancelButtonClicked(object? sender, EventArgs e) //async, da der bliver brugt await
        {
            bool confirm = await DisplayAlert("Annuller", "Udskyd 5 minutter", "Ja", "Nej");
            if (confirm)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
