using System.Diagnostics;
using tremorur.Models.Bluetooth;

namespace tremorur.Views;

public partial class MedicationAlarmPage : ContentPageWithButtons
{
    private readonly INavigationService navigationService;
    private readonly BluetoothService bluetoothService;
    private BluetoothPeripheral? peripheral;
    public MedicationAlarmPage(IButtonService buttonService, INavigationService navigationService, BluetoothService bluetoothService) : base(buttonService)
    {
        InitializeComponent();
        StartClock();
        this.navigationService = navigationService;
        this.bluetoothService = bluetoothService;
    }
    protected override void OnOKButtonClicked(object? sender, EventArgs e)
    {
        MedicationLabel.Text = "Medicinpåmindelse godkendt";
    }

    protected override void OnCancelButtonClicked(object? sender, EventArgs e)
    {
        MedicationLabel.Text = "Medicinpåmindelse annulleret";
        navigationService.GåTilSideAsync("//home");
    }

    protected override void OnDownButtonClicked(object? sender, EventArgs e)
    {
        bluetoothService.DiscoveredPeripheral += Discovered_PeripheralEvent;
        bluetoothService.StartDiscovery();
    }

    private async void Discovered_PeripheralEvent(object? sender, DiscoveredPeripheral discoveredPeripheral)
    {
        if (discoveredPeripheral.Name == "Askes fuckphone")
        {
            bluetoothService.StopDiscovery();
            bluetoothService.DiscoveredPeripheral -= Discovered_PeripheralEvent;
            try {
                peripheral = await bluetoothService.ConnectPeripheralAsync(discoveredPeripheral);
            } catch (Exception ex)
            {
                Debug.WriteLine($"Error connecting to peripheral: {ex.Message}");
                return;
            }
        }

        Debug.WriteLine($"Peripheral found: {discoveredPeripheral.RSSI}");
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