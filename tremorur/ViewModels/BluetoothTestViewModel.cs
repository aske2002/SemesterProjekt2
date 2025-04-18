using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using tremorur.Models.Bluetooth;

namespace tremorur.ViewModels
{
    public partial class BluetoothTestViewModel : ObservableObject
    {
        public ObservableCollection<DiscoveredPeripheral> Peripherals { get; } = new();

        [ObservableProperty]
        private BluetoothPeripheral? connectedDevice;

        private readonly BluetoothService bluetoothService;
        private readonly IDialogService dialogService;
        public BluetoothTestViewModel(BluetoothService bluetoothService, IDialogService dialogService) 
        {
            this.bluetoothService = bluetoothService;
            this.dialogService = dialogService;
            bluetoothService.DiscoveredPeripheral += OnDiscoveredPeripheral;
            bluetoothService.StartDiscovery();
        }

        private void OnDiscoveredPeripheral(object? sender, DiscoveredPeripheral e)
        {
            if (e.Name == null)
                return;

            Peripherals.Add(e);
        }

        [RelayCommand]
        public void Inspect()
        {
            Debug.WriteLine("Inspecting peripherals...");
        }

        [RelayCommand]
        public async Task Connect(DiscoveredPeripheral peripheral)
        {
            var device = await bluetoothService.ConnectPeripheralAsync(peripheral);
            ConnectedDevice = device;
        }

        public async Task ToggleNotify(BluetoothPeripheralCharacteristic characteristic, bool isEnabled)
        {
            if (ConnectedDevice == null)
                return;

            if (isEnabled)
            {
                await characteristic.SetNotifyingAsync(true);
            }
            else
            {
                await characteristic.SetNotifyingAsync(false);
            }
        }

        [RelayCommand]
        public async Task ReadCharacteristic(BluetoothPeripheralCharacteristic characteristic)
        {
            var data = await characteristic.ReadValueAsync();
            if (data != null)
            {
                Debug.WriteLine($"Read data: {BitConverter.ToString(data)}");
            }
            else
            {
                Debug.WriteLine("Failed to read data.");
            }
        }

        [RelayCommand]
        public async Task WriteCharacteristic(BluetoothPeripheralCharacteristic characteristic)
        {
            var data = await dialogService.DisplayPromptAsync("Write Data", "Enter data to write:", "OK", "Cancel");

            if (string.IsNullOrEmpty(data))
                return;

            await characteristic.WriteValueAsync(Encoding.UTF8.GetBytes(data));
        }
    }
}
