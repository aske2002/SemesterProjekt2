using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;

namespace tremorur.Views
{
    public partial class SetAlarmPage : ContentPageWithButtons
    {
        private readonly INavigationService navigationService;
        private readonly AlarmService alarmService;
        public SetAlarmPage(IButtonService buttonService, INavigationService navigationService, AlarmService alarmService) : base(buttonService)
        {
            InitializeComponent();
            this.navigationService = navigationService;
            UpdateAlarmLabel();
            this.alarmService = alarmService;
        }

        private int hours = 0;
        private int minutes = 0;
        private int step = 0; //0 vælg timer, 1 vælg minutter, 2 bekræft alarm, 3 gå til HomePage
        private int cancelPressCount = 0; //Tæller antal gange cancel bliver trykket
        protected override void OnUpButtonClicked(object? sender, EventArgs e)
        {
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
                minutes = (minutes - 5 + 60) % 60;
            }
            UpdateAlarmLabel();
        }
        private async Task SetStep(int _step)//returnerer en Task
        {
            step = _step;

            if (step == 0) // Timer indstilles
            {
                AlarmLabel.Text = "Indstil Timer";
            }
            else if (step == 1)
            {
                AlarmLabel.Text = "Indstil Minutter";
            }
            else if (step == 2)
            {
                AlarmLabel.Text = "Tryk OK for at gemme";
            }
            else if (step >= 2) //gemmer alarmen i alarmService og navigerer til HomePage
            {
                var time = TimeSpan.FromHours(hours).Add(TimeSpan.FromMinutes(minutes)); //laver om til timeSpan
                alarmService.CreateAlarm(time);
                await navigationService.GoToAsync("//home");
            }
        }
        protected override async void OnOKButtonClicked(object? sender, EventArgs e) //async, da der bliver brugt await
        {
            cancelPressCount = 0; //nulstiller når cancel bliver trykket
            await SetStep(step + 1);
        }
        protected override async void OnCancelButtonClicked(object? sender, EventArgs e) //async, da der bliver brugt await
        {
            cancelPressCount++;

            if (cancelPressCount>=2) //hvis cancel bliver trykket mere end 2 gange navigeres tilbage til HomePage
            {
                await navigationService.GoToAsync("//home");
            }
            else
            {
                SetStep(0); // Nulstil valg i alarmen
                hours = 0;
                minutes = 0;
                UpdateAlarmLabel();
            }
        }
        private void UpdateAlarmLabel() //opdaterer alarmLabel
        {
            SetAlarm.Text = $"{hours:D2}:{minutes:D2}";
        }
        
        protected override async void OnCancelButtonHeld(object? sender, int ms, Action didHandle)
        {
            if(ms > 5000)
            {
                didHandle();
                alarmService.ClearAlarms();
                Debug.WriteLine("Alle gemte alamer er slettet"); 
            }
            
        }
    }

}
