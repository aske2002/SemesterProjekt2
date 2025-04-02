namespace tremorur.Views
{
    public partial class SetAlarmPage : ContentPage
    {
        public SetAlarmPage(SetAlarmViewModel viewModel)
        {
            InitializeComponent();
            UpdateAlarmLabel();
        }
        private int hours = 0;
        private int minutes = 0;
        private int step = 0; //0 vælg timer, 1 vælg minutter, 2 bekræft alarm

        private void OnUpClicked(object sender, EventArgs e)
        {
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

        private void OnDownClicked(object sender, EventArgs e)
        {
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

        private void OnOkClicked(object sender, EventArgs e)
        {
            if (step == 0) // Timer indstilles
            {
                step = 1;
            }
            else if (step == 1) //Minutter indstilles
            {
                step = 2;
            }
            else if (step == 2) // Alarmen gemmes
            {
                DisplayAlert("Alarm sat", $"Alarm sat til {hours:D2}:{minutes:D2}", "OK");
                step = 0; // Nulstil valgprocessen
            }
            UpdateAlarmLabel();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            step = 0; // Nulstil valg
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
