using Microsoft.Extensions.Logging;
using shared.Models;
using shared.Models.Vibrations;
using tremorur.Models.Bluetooth;
using tremorur.Models.Bluetooth.Events;


namespace tremorur.Services
{
    public class VibrationsService
    {
        private readonly IBluetoothStateManager _bluetoothStateManager;
        private IBluetoothPeripheralService? _vibrationService => _bluetoothStateManager.Peripheral?.Services?.FirstOrDefault(e => e.UUID == BluetoothIdentifiers.TemorurServiceUUID);
        private IBluetoothPeripheralCharacteristic? _patternChar => _vibrationService?.Characteristics?.FirstOrDefault(e => e.UUID == BluetoothIdentifiers.VibrationPatternCharacteristicUUID);
        private IBluetoothPeripheralCharacteristic? _onOffChar => _vibrationService?.Characteristics?.FirstOrDefault(e => e.UUID == BluetoothIdentifiers.VibrationEnabledCharacteristicUUID);
        private ILogger<VibrationsService> logger;

        public event EventHandler<int>? VibrationLevelChanged; //event der sender vibration level til UI
        public event EventHandler<bool>? VibrationEnabledStateChanged; //event der sender vibration enabled state til UI

        public VibrationsService(IBluetoothStateManager bluetoothStateManager, ILogger<VibrationsService> logger) //initialiserer bluetoothStateManager
        {
            _bluetoothStateManager = bluetoothStateManager;
            _bluetoothStateManager.CharacteristicValueChanged += CharacteristicValueChanged; //tilføjer event handler til bluetoothStateManager
            _bluetoothStateManager.DiscoveredCharacteristic += DiscoveredCharacteristic; //tilføjer event handler til bluetoothStateManager
            this.logger = logger;
        }

        private int currentLevelIndex = 0; //holder styr på nuværende vibrations level
        public int CurrentLevelIndex
        {
            get => currentLevelIndex;
            private set
            {
                if (currentLevelIndex != value)
                {
                    currentLevelIndex = value;
                    VibrationLevelChanged?.Invoke(this, currentLevelIndex); //sender vibration level til UI
                }
            }
        }

        private bool isEnabled = false; //holder styr på om vibration er tændt eller slukket
        public bool IsEnabled
        {
            get => isEnabled;
            private set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    VibrationEnabledStateChanged?.Invoke(this, isEnabled); //sender vibration enabled state til UI
                }
            }
        }

        private async void DiscoveredCharacteristic(object? sender, DiscoveredCharacteristicEventArgs e)
        {
            if
            (
                e.Characteristic.UUID == BluetoothIdentifiers.VibrationPatternCharacteristicUUID ||
                e.Characteristic.UUID == BluetoothIdentifiers.VibrationEnabledCharacteristicUUID
            )
            {
                CharacteristicValueChanged(sender, new CharacteristicValueChangedEventArgs(e.Peripheral, e.Service, e.Characteristic, await e.Characteristic.ReadValueAsync())); //læser nuværende vibrations level og enabled state
            }
        }

        private async void CharacteristicValueChanged(object? sender, CharacteristicValueChangedEventArgs e)
        {
            switch (e.Characteristic.UUID)
            {
                case BluetoothIdentifiers.VibrationPatternCharacteristicUUID:
                    CurrentLevelIndex = await GetCurrentVibration(e.Value); //henter nuværende vibrations level
                    logger.LogInformation("Vibration level changed to {level}", CurrentLevelIndex);
                    break;
                case BluetoothIdentifiers.VibrationEnabledCharacteristicUUID:
                    IsEnabled = e.Value.FirstOrDefault() == 1; //henter nuværende vibrations enabled state
                    logger.LogInformation("Vibration enabled state changed to {state}", IsEnabled);
                    break;
            }
        }

        public async Task StartStopVibration()
        {
            if (_onOffChar == null)//hvis bluetooth ikke er forbundet stopper metoden
            {
                return;
            }
            var currentValue = await _onOffChar.ReadValueAsync(); //læser fra RPi om vibration er tændt
            logger.LogInformation("Toggling vibration, current state: {state}", currentValue.FirstOrDefault());//logbeked med informationsniveau

            var vibrationRunning = currentValue.FirstOrDefault() == 1; //hvis vibration er tændt [1] så er vibrationRunning true, ellers false

            if (!vibrationRunning)
            {
                await _onOffChar.WriteValueAsync([1]);//hvis vibration er ikke er tændt [0], så tændes den
                if (_patternChar == null)//hvis bluetooth ikke er forbundet stopper metoden
                {
                    return;
                }
                await _patternChar.WriteValueAsync(vibrationsLevels[0].ToBytes()); //sætter vibrations level til 1
            }
            else
            {
                await _onOffChar.WriteValueAsync([0]);//slukker hvis vibration er tændt [1]
                CurrentLevelIndex = 0; //sætter vibrations level til 0
            }
        }
        private static readonly List<VibrationSettings> vibrationsLevels = new List<VibrationSettings>()
        {
            VibrationSettings.CreateSinePatternSettings(0.5), //level 1
            VibrationSettings.CreateSinePatternSettings(5), // level 2
            VibrationSettings.CreateDynamicPatternSettings((1000,1),(500,0),(500,1),(700,0)),//level 3
            VibrationSettings.CreateDynamicPatternSettings((2000,1),(750,0),(1000,1)), //level 4
            VibrationSettings.CreateMixedPatternSettings(
                (VibrationSettings.CreateSinePatternSettings(80), 2000), //sinus 2 sekunder
                (VibrationSettings.CreateDynamicPatternSettings((2000,1),(750,0)), 3000)// dynamik 3 sekunder
            ), // Level 5
            VibrationSettings.CreateMixedPatternSettings(
                (VibrationSettings.CreateSinePatternSettings(1), 2000), // sinus i 2 sekunder
                (VibrationSettings.CreateConstantPatternSettings(0.7), 3000) // derefter konstant 70% intensitet i 3 sekunder
            ), //level 6
            VibrationSettings.CreateConstantPatternSettings(1), //level 7
        };

        public async Task<int> GetCurrentVibration(byte[]? data = null)//hjælpemetode der finder nuværende level
        {
            if (data == null)
            {
                if (_patternChar == null)
                {
                    return 0;
                }

                data = await _patternChar.ReadValueAsync(); //læser vibrationsmønster fra RPi hvis tændt
            }

            var currentPattern = await VibrationSettings.FromBytes(data);//identificerer mønsteret
            var currentPatternInList = vibrationsLevels.FirstOrDefault(e => e.Id == currentPattern?.Id); //finder mønsteret i vibrations liste

            if (currentPatternInList != null)
            {
                var currentLevel = vibrationsLevels.IndexOf(currentPatternInList);
                return currentLevel >= 0 ? currentLevel : 0; //returnerer level som 1-7
            }

            return 0;//ukendt mønster
        }
        public async Task NavigateLevelUp()
        {
            if (_patternChar == null)
            {
                return;
            }
            var nextLevelIndex = (currentLevelIndex + 1 + vibrationsLevels.Count) % vibrationsLevels.Count; //finder næste index i vibrations liste
            logger.LogInformation("Navigating to level {level}", nextLevelIndex);
            var nextLevel = vibrationsLevels[nextLevelIndex];//finder næste objekt i vibrations liste
            await _patternChar.WriteValueAsync(nextLevel.ToBytes());//skriver level om til bytes og sender det til RPi
        }
        public async Task NavigateLevelDown()
        {
            if (_patternChar == null)
            {
                return;
            }
            var preLevelIndex = (currentLevelIndex - 1 + vibrationsLevels.Count) % vibrationsLevels.Count; //finder forrige index i vibrations liste
            var preLevel = vibrationsLevels[preLevelIndex];//finder forrige objekt i vibrations liste
            await _patternChar.WriteValueAsync(preLevel.ToBytes());//skriver level om til bytes og sender det til RPi
        }
    }
}
