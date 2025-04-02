namespace tremorur.Views;

public partial class MedicationAlarmPage : ContentPage
{
	public MedicationAlarmPage()
	{
		InitializeComponent();
        StartClock();
	}
    async void StartClock()
    {
        while (true)
        {
            DateTime now = DateTime.Now;
            TimeSpan currentTime = now.TimeOfDay; // Henter tidspunktet som TimeSpan

            //ClockLabel.Text = $"{currentTime.Hours:D2}:{currentTime.Minutes:D2}"; // Viser tid i timer og sekunder

            await Task.Delay(1000); // Opdater hvert sekund
        }
    }


}