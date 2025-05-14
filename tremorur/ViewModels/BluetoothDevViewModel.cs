using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Input;
using shared.Models;
using shared.Models.Vibrations;
using shared.Models.Vibrations.Patterns;
using tremorur.Models.Bluetooth;

namespace tremorur.ViewModels
{
    [QueryProperty(nameof(ConnectedDevice), "ConnectedDevice")]
    public partial class BluetoothDevViewModel : INotifyPropertyChanged
    {
        BluetoothPeripheral? connectedDevice;
        public INavigationService _navigationService { get; set; }
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

        public BluetoothDevViewModel(IBluetoothService bluetoothService, IDialogService dialogService, INavigationService navigationService)
        {
            this.bluetoothService = bluetoothService;
            this.dialogService = dialogService;
            this._navigationService = navigationService;
            EntryCompletedCommand = new Command<string>(SendTextPattern);

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

        [RelayCommand]
        public async Task Send1()
        {
            var pattern = VibrationSettings.CreateDynamicPatternSettings((500, 1.0), (500, 0.5), (500, 0.0)).Pattern;
            await SendVibrationPattern(pattern);
        }

        [RelayCommand]
        public async Task Send2()
        {
            var pattern = VibrationSettings.CreateSinePatternSettings(1).Pattern;
            await SendVibrationPattern(pattern);
        }

        [RelayCommand]
        public async Task Send3()
        {
            var pattern = VibrationSettings.CreateConstantPatternSettings(0.5).Pattern;
            await SendVibrationPattern(pattern);
        }

        [RelayCommand]
        public async Task Send4()
        {
            var pattern = VibrationSettings.CreateDynamicPatternSettings((500, 1.0), (250, 0.0), (250, 0.0)).Pattern;
            await SendVibrationPattern(pattern);

        }
        public ICommand EntryCompletedCommand { get; }
        public async void SendTextPattern(string text)
        {
            try
            {
                var pattern = await VibrationPatternExpression.ParseAsync(text, 1);
                await SendVibrationPattern(pattern);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error parsing pattern: {ex.Message}");
                return;
            }

        }


        public async Task SendVibrationPattern(IVibrationPattern pattern)
        {
            var settings = new VibrationSettings
            {
                Id = Guid.NewGuid(),
                Pattern = pattern
            };
            var characteristic = connectedDevice?.Services
                .FirstOrDefault(s => s.UUID.Equals(BluetoothIdentifiers.VibrationServiceUUID, StringComparison.OrdinalIgnoreCase))?.Characteristics
                .FirstOrDefault(c => c.UUID.Equals(BluetoothIdentifiers.VibrationPatternCharacteristicUUID, StringComparison.OrdinalIgnoreCase));

            if (characteristic != null)
            {
                await characteristic.WriteValueAsync(settings.ToBytes());
            }
            else
            {
                Debug.WriteLine("Characteristic not found");
            }
        }

        [RelayCommand]
        public async Task GoBack()
        {
            bluetoothService.StartDiscovery(BluetoothIdentifiers.VibrationServiceUUID);
            await _navigationService.GoToAsync("///bluetoothConnect");
        }

        [RelayCommand]
        public async Task ToggleVibrations()
        {
            var characteristic = connectedDevice?.Services.FirstOrDefault(s => s.UUID.Equals(BluetoothIdentifiers.VibrationServiceUUID, StringComparison.OrdinalIgnoreCase))?.Characteristics
                .FirstOrDefault(c => c.UUID.Equals(BluetoothIdentifiers.VibrationEnabledCharacteristicUUID, StringComparison.OrdinalIgnoreCase));
            if (characteristic != null)
            {
                var currentVal = await characteristic.ReadValueAsync();
                var isOn = currentVal.Length > 0 && currentVal.First() == 1;
                Debug.WriteLine(isOn ? "Turning off vibrations" : "Turning on vibrations");
                await characteristic.WriteValueAsync(isOn ? [0] : [1]);
            }
            else
            {
                Debug.WriteLine("Characteristic not found");
            }
        }
    }
}
