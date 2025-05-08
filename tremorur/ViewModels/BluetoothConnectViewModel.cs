using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using tremorur.Models.Bluetooth;

namespace tremorur.ViewModels
{
    public partial class BluetoothConnectViewModel : ObservableObject
    {
        public ObservableCollection<IDiscoveredPeripheral> Peripherals { get; } = new();

        private readonly IBluetoothService bluetoothService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        public BluetoothConnectViewModel(IBluetoothService bluetoothService, IDialogService dialogService, INavigationService navigationService)
        {
            this.bluetoothService = bluetoothService;
            this.navigationService = navigationService;
            this.dialogService = dialogService;
            bluetoothService.DiscoveredPeripheral += OnDiscoveredPeripheral;
            bluetoothService.StartDiscovery();
        }

        private void OnDiscoveredPeripheral(object? sender, IDiscoveredPeripheral e)
        {
            if (e.Name == null)
                return;

            Peripherals.Add(e);
        }

        [RelayCommand]
        public async Task Connect(IDiscoveredPeripheral peripheral)
        {
            var device = await bluetoothService.ConnectPeripheralAsync(peripheral);
            await navigationService.GoToAsync("///bluetoothDev", new Dictionary<string, object>
            {
                { "ConnectedDevice", device }
            });
        }
    }
}
