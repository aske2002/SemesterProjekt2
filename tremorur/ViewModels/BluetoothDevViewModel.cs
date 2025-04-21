using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using tremorur.Models.Bluetooth;

namespace tremorur.ViewModels
{
    [QueryProperty(nameof(ConnectedDevice), "ConnectedDevice")]
    public partial class BluetoothDevViewModel : ObservableObject
    {
        IBluetoothPeripheral? connectedDevice;
        public IBluetoothPeripheral? ConnectedDevice
        {
            get => connectedDevice;
            set
            {
                connectedDevice = value;
                OnPropertyChanged();
            }
        }

        private readonly BluetoothService bluetoothService;
        private readonly IDialogService dialogService;
        public BluetoothDevViewModel(BluetoothService bluetoothService, IDialogService dialogService)
        {
            this.bluetoothService = bluetoothService;
            this.dialogService = dialogService;
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
