using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace tremorur.Views
{
    public partial class SetAlarmPage : ContentPageWithButtons
    {
        private readonly INavigationService navigationService;
        public SetAlarmPage(IButtonService buttonService, INavigationService navigationService) : base(buttonService)
        {
            InitializeComponent();
            this.navigationService = navigationService;
            UpdateAlarmLabel();
        }

        private int hours = 0;
        private int minutes = 0;
        private int step = 0; //0 vælg timer, 1 vælg minutter, 2 bekræft alarm, 3 gå til HomePage
        private int cancelPressCount = 0; //Tæller antal gange cancel bliver trykket
        protected override void OnUpButtonClicked(object? sender, EventArgs e)
        {
            AlarmLabel.Text = "Indstil Alarm";
            cancelPressCount = 0; //nulstiller når cancel bliver trykket
            if (step == 0) // Justerer timer
            {
                hours = (hours + 1) % 24;
            }
            else if (step == 1) // Justerer minutter
            {
                minutes = (minutes + 1) % 60;
            }
            UpdateAlarmLabel();
        }
        protected override void OnDownButtonClicked(object? sender, EventArgs e)
        {
            cancelPressCount = 0; //nulstiller når cancel bliver trykket
            if (step == 0) // Justerer timer
            {
                hours = (hours - 1 + 24) % 24;
            }
            else if (step == 1) // Justerer minutter
            {
                minutes = (minutes - 1 + 60) % 60;
            }
            UpdateAlarmLabel();
        }
        protected override async void OnOKButtonClicked(object? sender, EventArgs e) //async, da der bliver brugt await
        {
            cancelPressCount = 0; //nulstiller når cancel bliver trykket
            if (step == 0) // Timer indstilles
            {
                AlarmLabel.Text = "Indstil timer";
                step = 1;
            }
            else if (step == 1) //Minutter indstilles
            {
                AlarmLabel.Text = "Indstil minutter";
                step = 2;
            }
            else if (step == 2) // Alarmen gemmes
            {
                await DisplayAlert("Alarm sat", $"Alarm sat til {hours:D2}:{minutes:D2}", "OK");
                step = 0; // Nulstil valgprocessen
                hours = 0;
                minutes = 0;
                await navigationService.GoToAsync("//home"); //navigerer til HomePage
                return; //stopper koden i metoden, så man kan navigere væk
            }
                UpdateAlarmLabel();
            
        }
        protected override async void OnCancelButtonClicked(object? sender, EventArgs e) //async, da der bliver brugt await
        {
            cancelPressCount++;

            if (cancelPressCount>=2) //hvis cancel bliver trykket mere end 2 gange navigeres tilbage til HomePage
            {
                cancelPressCount = 0;
                step = 0;
                hours = 0;
                minutes = 0;
                await navigationService.GoToAsync("//home");
                return;
            }

            step = 0; // Nulstil valg i alarmen
            hours = 0;
            minutes = 0;
            UpdateAlarmLabel();
        }
        private void UpdateAlarmLabel()
        {
            SetAlarm.Text = $"Alarm: {hours:D2}:{minutes:D2}";
        }
    }

}
