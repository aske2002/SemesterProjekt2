using System.ComponentModel;
using System.Diagnostics;
using System.Speech.Recognition;
using tremorur.Models.Bluetooth;

namespace tremorur.Views;
public partial class MedicationAlarmPage : ContentPageWithButtons
{
    private readonly INavigationService navigationService;
    private readonly AlarmService alarmService;

    private Alarm? alarmToShow;
    public MedicationAlarmPage(IButtonService buttonService, INavigationService navigationService, AlarmService alarmService) : base(buttonService)
    {
        InitializeComponent();
        StartClock();
        this.navigationService = navigationService;
        this.alarmService = alarmService;
    }
    protected override void OnAppearing() //alarmen håndteres gennem metode 
    {
        alarmToShow = alarmService.CurrentAlarm; //henter aktuelle alarm fra AlarmServive
        if (alarmToShow !=null)
        {
            MedicationLabel.Text = $"Tag din medicin! ({alarmToShow.TimeSpan})"; //viser alarmtid
        }
        else 
        {
            MedicationLabel.Text = "Ingen gemte alarmer!";
        }
        base.OnAppearing(); //kalder base-implementation
    }
    protected override async void OnOKButtonClicked(object? sender, EventArgs e)
    {
        MedicationLabel.Text = "Medicinpåmindelse godkendt";
        if (alarmToShow != null) 
        {
            RegisterResponse(alarmToShow, true); //registerer at brugeren godkender
        }
        await Task.Delay(3000); //venter 3 sekunder med at navigerer til HomePage
        await navigationService.GoToAsync("//home");
    }
    protected override async void OnCancelButtonClicked(object? sender, EventArgs e)  
    {
        MedicationLabel.Text = "Medicinpåmindelse annulleret";
        if (alarmToShow != null)
        {
            RegisterResponse(alarmToShow, false); //registerer at brugeren annullerer
        }
        await Task.Delay(3000); //venter 3 sekunder med at navigerer til HomePage
        await navigationService.GoToAsync("//home");
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
    public void RegisterResponse(Alarm alarm, bool accepted) //registerer brugerens respons
    {
        if (accepted)   
        {
            Debug.WriteLine($"Alarm {alarm.Id} blev godkendt");
        }
        else
        {
            Debug.WriteLine($"Alarm {alarm.Id} blev afvist");
        }
    }
}