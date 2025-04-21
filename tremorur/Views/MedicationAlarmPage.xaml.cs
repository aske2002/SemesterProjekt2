using System.Diagnostics;
using tremorur.Models.Bluetooth;

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
    protected override void OnOKButtonClicked(object? sender, EventArgs e)
    {
        MedicationLabel.Text = "Medicinpåmindelse godkendt";
    }

    protected override void OnCancelButtonClicked(object? sender, EventArgs e)  
    {
        MedicationLabel.Text = "Medicinpåmindelse annulleret";
        navigationService.GoToAsync("//home");
    }
    protected override void OnDownButtonClicked(object? sender, EventArgs e)
    {

    }

    protected override void OnUpButtonClicked(object? sender, EventArgs e)
    {
        MedicationLabel.Text = "Op blev trykket";
    }
    //protected override async void OnCancelButtonClicked(object? sender, EventArgs e) //async, da der bliver brugt await
    //{
    //    bool confirm = await DisplayAlert("Annuller", "Udskyd 5 minutter", "Ja", "Nej");
    //    if (confirm)
    //    {
    //        await Navigation.PopAsync();
    //    }
    //}
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


}