using System.ComponentModel;
using System.Diagnostics;
using System.Security.Claims;
using tremorur.Models;
using tremorur.Models.Bluetooth;

namespace tremorur.Views;
[QueryProperty(nameof(AlarmId), "alarmId")]
public partial class MedicationAlarmPage : ContentPageWithButtons
{
    private readonly INavigationService navigationService;
    private readonly AlarmService alarmService;

    private Alarm? alarmToShow;
    public MedicationAlarmPage(MedicationAlarmViewModel viewModel, IButtonService buttonService, INavigationService navigationService, AlarmService alarmService) : base(buttonService)
    {
        BindingContext = viewModel;
        InitializeComponent();
        StartClock();
        this.navigationService = navigationService;
        this.alarmService = alarmService;
    }
    public string AlarmId //QueryProperty, som sættes af MAUI-shell, når alarmId sendes via navigationen
    {
        set
        {
            var alarm = alarmService.GetAlarm(value); // Hent alarm baseret på id (value) fra alarmService
            if (alarm != null)
            {
                alarmToShow = alarm; //hvis alarmen findes gemmes den lokalt i AlarmToShow og siden opdaterer medicationLabel
                MedicationLabel.Text = "Tag din medicin!";
            }
            else
            {
                MedicationLabel.Text = "Kunne ikke hente alarm";
            }
        }
    }
    protected override async void OnOKButtonClicked(object? sender,EventArgs e)
    {
        MedicationLabel.Text = "Påmindelse godkendt";
        if (alarmToShow != null) 
        { 
            RegisterResponse(alarmToShow,true);//registerer at brugeren godkender
        }
        await Task.Delay(3000);
        await navigationService.GoToAsync("//home");//venter 3 sekunder med at navigerer til HomePage
    }

    protected override async void OnCancelButtonClicked(object? sender, EventArgs e)
    {
        MedicationLabel.Text = "Påmindelse annulleret";
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
    public void RegisterResponse(Alarm alarm, bool accepted) //registerer brugerens respons i debug med dato og alarm-id
    {
        DateTime now = DateTime.Now;
        TimeSpan currentTime = now.TimeOfDay;
        string date = now.ToString("dd.MM.yyyy");
        if (accepted)
        {
            Debug.WriteLine($"Alarm {alarm.Id} blev godkendt {alarm.TimeSpan} {date}");
        }
        else
        {
            Debug.WriteLine($"Alarm {alarm.Id} blev afvist {alarm.TimeSpan} {date}");
        }
    }
}