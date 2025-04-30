using System.Diagnostics;

namespace tremorur.Views;

public partial class MedicationAlarmPage : ContentPageWithButtons
{
    private readonly INavigationService navigationService;
    private readonly AlarmService alarmService;
    public MedicationAlarmPage(IButtonService buttonService, INavigationService navigationService, AlarmService alarmService) : base(buttonService)
    {
        InitializeComponent();
        StartClock();
        this.navigationService = navigationService;
        this.alarmService = alarmService;
    }
    public Alarm Alarm {  get; }

    protected void AlarmAppearing()
    {
        if (Alarm != null)
        {
            MedicationLabel.Text = "Tag din medicin";
            //godkend afvis
        }
    }
    protected override async void OnOKButtonClicked(object? sender, EventArgs e)
    {
        MedicationLabel.Text = "Medicinpåmindelse godkendt";
        await Task.Delay(3000); //venter 3 sekunder med at navigerer til hjemmeskærm
        await navigationService.GoToAsync("//home");
    }
    public int cancelPressCount;
    protected override async void OnCancelButtonClicked(object? sender, EventArgs e)  
    {
        cancelPressCount++;
        
        if (cancelPressCount >= 2) //hvis cancel bliver trykket mere end 2 gange navigeres tilbage til HomePage
        {
            MedicationLabel.Text = "Medicinpåmindelse annulleret";
            await Task.Delay(3000); //venter 3 sekunder med at navigerer til hjemmeskærm
            await navigationService.GoToAsync("//home");
        }
    }
    async void StartClock()
    {
        while (true)
        {
            DateTime now = DateTime.Now;
            TimeSpan currentTime = now.TimeOfDay; // Henter tidspunktet som TimeSpan
            ClockLabel.Text = $"{currentTime.Hours:D2}:{currentTime.Minutes:D2}"; // Viser tid i timer og sekunder
            await Task.Delay(1000); // Opdater hvert sekund
        }
    }

    public void RegisterResponse(Alarm alarm, bool accepted)
    {
        if (accepted)
        {
            Debug.WriteLine($"Alarm blev godkendt");
        }
        else
        {
            Debug.WriteLine($"Alarm blev afvist");
        }
    }
}