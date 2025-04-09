namespace tremorur.Views;

public partial class MedicationAlarmPage : ContentPageWithButtons
{
    private readonly INavigationService navigationService;
    public MedicationAlarmPage(IButtonService buttonService, INavigationService navigationService) : base(buttonService)
    {
        InitializeComponent();
        StartClock();
        this.navigationService = navigationService;
    }

    protected override void OnButtonClicked(object? sender, EventArgs e)
    {
        MedicationLabel.Text = "Medicinpåmindelse blev godkendt";
    }

    protected override void OnCancelButtonClicked(object? sender, EventArgs e)
    {
        navigationService.GåTilSideAsync("//home");
    }

    protected override void OnDownButtonClicked(object? sender, EventArgs e)
    {
        MedicationLabel.Text = "Ned blev trykket";
    }

    protected override void OnUpButtonClicked(object? sender, EventArgs e)
    {
        MedicationLabel.Text = "Op blev trykket";
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