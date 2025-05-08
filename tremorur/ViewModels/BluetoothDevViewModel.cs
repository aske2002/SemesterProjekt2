using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using tremorur.Models.Bluetooth;

namespace tremorur.ViewModels
{
    [QueryProperty(nameof(ConnectedDevice), "ConnectedDevice")]
    public partial class BluetoothDevViewModel : INotifyPropertyChanged
    {
        BluetoothPeripheral? connectedDevice;
        public BluetoothPeripheral? ConnectedDevice
        {
            get => connectedDevice;
            set
            {
                connectedDevice = value;

                if (value != null)
                {
                    SubscribeToCharacteristic(value);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConnectedDevice)));
            }
        }

        private readonly IBluetoothService bluetoothService;
        private readonly IDialogService dialogService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public BluetoothDevViewModel(IBluetoothService bluetoothService, IDialogService dialogService)
        {
            this.bluetoothService = bluetoothService;
            this.dialogService = dialogService;
        }

        private void SubscribeToCharacteristic(IBluetoothPeripheral peripheral)
        {
            foreach (var service in peripheral.Services)
            {
                SubscribeToService(service);
            }
            peripheral.Services.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
                {
                    foreach (BluetoothPeripheralService service in e.NewItems)
                    {
                        SubscribeToService(service);
                    }
                }
            };
        }

        private void SubscribeToService(IBluetoothPeripheralService service)
        {
            foreach (var characteristic in service.Characteristics)
            {
                SubscribeToCharacteristic(characteristic, service);
            }
            service.Characteristics.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
                {
                    foreach (BluetoothPeripheralCharacteristic characteristic in e.NewItems)
                    {
                        SubscribeToCharacteristic(characteristic, service);
                    }
                }
            };
        }

        private void SubscribeToCharacteristic(IBluetoothPeripheralCharacteristic characteristic, IBluetoothPeripheralService service)
        {
            var action = () =>
            {
                var characteristicElement = Shell.Current.CurrentPage.GetVisualTreeDescendants().OfType<BindableObject>().FirstOrDefault(x => x.BindingContext == characteristic && x is Grid) as Grid;
                var switchElement = characteristicElement.FindByName<Microsoft.Maui.Controls.Switch>("NotifySwitch");
                var valueLabel = characteristicElement.FindByName<Label>("ValueLabel");

                if (switchElement != null)
                {
                    switchElement.IsToggled = characteristic.IsNotifying;
                }

                if (valueLabel != null)
                {
                    valueLabel.Text = characteristic.LastValueString;
                }
            };


            characteristic.ValueUpdated += (s, e) => action();
            characteristic.NotifyingUpdated += (s, e) => action();
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
