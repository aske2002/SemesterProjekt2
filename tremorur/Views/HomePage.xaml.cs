using System.Reflection;

namespace tremorur.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();
            StartClock(); //starter uret

        }
        private async void StartClock()
        {
            while (true)
            {
                ClockLabel.Text = DateTime.Now.ToString("HH:mm"); //Opdater label med klokken. Henter den aktuelle tid i 24 timers format
                await Task.Delay(1000); //sørger for at opdateringen sker hvert sekund
            }
        }
    }
}
