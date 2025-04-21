using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shared.Models;
using shared.Models.Vibrations;
using tremorur.Models.Bluetooth;


namespace tremorur.Services
{
    public class VibrationsService
    {
        private readonly IBluetoothService _bluetoothService;
        private IBluetoothPeripheral? _connectedDevice = null; //null indtil der forbindes til bluetoothEnheden 
        private IBluetoothPeripheralService? _vibrationService =>_connectedDevice?.Services?.FirstOrDefault(e => e.UUID == BluetoothID.VibrationServiceUUID);
        private IBluetoothPeripheralCharacteristic? _patternChar => _vibrationService?.Characteristics?.FirstOrDefault(e => e.UUID == BluetoothID.VibrationPatternCharacteristicUUID);
        private IBluetoothPeripheralCharacteristic? _onOffChar => _vibrationService?.Characteristics?.FirstOrDefault(e => e.UUID == BluetoothID.VibrationEnabledCharacteristicUUID);

        public VibrationsService(IBluetoothService bluetoothService)//initialiserer bluetoothService
        {
            _bluetoothService = bluetoothService;
            ConnectBluetooth();
        }

        private void ConnectBluetooth()
        {
            _bluetoothService.DiscoveredPeripheral += DiscoveredDevice;
        }

        private async void DiscoveredDevice(object? sender, IDiscoveredPeripheral device)
        {
            if (device.Name == "Tremorur")
            {
                _connectedDevice = await _bluetoothService.ConnectPeripheralAsync(device);
            }
        }

        public async Task StartStopVibration()
        {
            if (_onOffChar == null)//hvis bluetooth ikke er forbundet stopper metoden
            {
                return;
            }

            var currentValue = await _onOffChar.ReadValueAsync(); //læser fra RPi om vibration er tændt
            var vibrationRunning = currentValue[0] == 1;//checker om vibration er tændt

            if (!vibrationRunning)
            {
                await _onOffChar.WriteValueAsync([1]);//hvis vibration er ikke er tændt [0], så tændes den
            }
            else
            {
                await _onOffChar.WriteValueAsync([0]);//slukker hvis vibration er tændt [1]
            }
        }
        private static readonly List<VibrationSettings> vibrationsLevels = new List<VibrationSettings>()
        {
            VibrationSettings.CreateSinePatternSettings(50), //level 1
            VibrationSettings.CreateSinePatternSettings(80), // level 2
            VibrationSettings.CreateDynamicPatternSettings((1000,1),(100,0),(500,1),(150,0)),//level 3
            VibrationSettings.CreateSinePatternSettings(50), //level 4
            VibrationSettings.CreateSinePatternSettings(50), //level 5
            VibrationSettings.CreateSinePatternSettings(50), //level 6
            VibrationSettings.CreateConstantPatternSettings(1), //level 7
        };

        private async Task<int> GetCurrentVibration(IBluetoothPeripheralCharacteristic characteristic)//metode for at gøre navigation lettere
        {
            var currentData = await characteristic.ReadValueAsync(); //læser fra PPi hvad mønsteret er
            var currentPattern = await VibrationSettings.FromBytes(currentData);//læser hvilket level mønster RPi er på

            var currentLevel = 0;
            if (currentPattern != null)
            {
                currentLevel = vibrationsLevels.IndexOf(vibrationsLevels.Find(e => e.Id == currentPattern.Id));
            }
            return currentLevel;
        }
        public async Task NavigateLevelUp()
        {
            if (_patternChar == null)
            {
                return;
            }
            var currentVibrationIndex = await GetCurrentVibration(_patternChar);
            var nextLevelIndex = (currentVibrationIndex + 1) % vibrationsLevels.Count; //finder næste index i vibrations liste
            var nextLevel = vibrationsLevels[nextLevelIndex];//finder næste objekt i vibrations liste
            await _patternChar.WriteValueAsync(nextLevel.ToBytes());//skriver level om til bytes og sender det til RPi
        }
        public async Task NavigateLevelDown()
        {
            if (_patternChar == null)
            {
                return;
            }
            var currentVibrationIndex = await GetCurrentVibration(_patternChar);
            var preLevelIndex = (currentVibrationIndex - 1) % vibrationsLevels.Count; //finder forrige index i vibrations liste
            var preLevel = vibrationsLevels[preLevelIndex];//finder forrige objekt i vibrations liste
            await _patternChar.WriteValueAsync(preLevel.ToBytes());//skriver level om til bytes og sender det til RPi
        }

    }
}
