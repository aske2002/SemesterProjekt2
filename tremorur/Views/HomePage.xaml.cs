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

        private CancellationTokenSource? okHoldCts;//hold ok nede
        private CancellationTokenSource? upHoldCts;//hold up nede

        protected override void OnOKButtonClicked(object? sender, EventArgs e)
        {
            if (okHoldCts != null)
            { 
                okHoldCts.Cancel();//hvis knappen trykkes igen afbrydes der
                okHoldCts = null;
                return;
            }
            okHoldCts = new CancellationTokenSource();
            StartOkHoldTimer(okHoldCts.Token);
        }
        private async void StartOkHoldTimer(CancellationToken token)
        {
            try
            {
                await Task.Delay(3000, token); //hold nede i 3 sekunder
                await vibrationsService.StartStopVibration(); //starter vibrationer
                await navigationService.GoToAsync("//setVibration"); //går til SetVibrationsPage
            }
            catch(TaskCanceledException){ }//der slippes inden 3 sekunder - afbryd   
            finally { okHoldCts= null; }
        }

        protected override void OnUpButtonClicked(object? sender, EventArgs e)
        {
            if (upHoldCts != null)
            {
                upHoldCts.Cancel();//hvis knappen trykkes igen afbrydes der
                upHoldCts = null;
                return;
            }
            okHoldCts = new CancellationTokenSource();
            StartUpHoldTimer(upHoldCts.Token);
        }
        private async void StartUpHoldTimer(CancellationToken token)
        {
            try
            {
                await Task.Delay(3000, token); //hold nede i 3 sekunder
                await navigationService.GoToAsync("//setAlarm"); //går til SetAlarmPage
            }
            catch (TaskCanceledException) { }//der slippes inden 3 sekunder - afbryd   
            finally { upHoldCts = null; }
        }
    }
}
