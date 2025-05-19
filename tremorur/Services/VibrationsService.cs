using Microsoft.Extensions.Logging;
using shared.Models;
using shared.Models.Vibrations;
using tremorur.Models.Bluetooth;


namespace tremorur.Services
{
    public class VibrationsService
    {
        private readonly IBluetoothStateManager _bluetoothStateManager;
        private IBluetoothPeripheralService? _vibrationService => _bluetoothStateManager.Peripheral?.Services?.FirstOrDefault(e => e.UUID == BluetoothIdentifiers.VibrationServiceUUID);
        private IBluetoothPeripheralCharacteristic? _patternChar => _vibrationService?.Characteristics?.FirstOrDefault(e => e.UUID == BluetoothIdentifiers.VibrationPatternCharacteristicUUID);
        private IBluetoothPeripheralCharacteristic? _onOffChar => _vibrationService?.Characteristics?.FirstOrDefault(e => e.UUID == BluetoothIdentifiers.VibrationEnabledCharacteristicUUID);
        private ILogger<VibrationsService> logger;

        public VibrationsService(IBluetoothStateManager bluetoothStateManager, ILogger<VibrationsService> logger) //initialiserer bluetoothService
        {
            _bluetoothStateManager = bluetoothStateManager;
            this.logger = logger;
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
            VibrationSettings.CreateDynamicPatternSettings((1000,1),(500,0),(500,1),(700,0)),//level 3
            VibrationSettings.CreateDynamicPatternSettings((2000,1),(750,0),(1000,1)), //level 4
            VibrationSettings.CreateExpressionSettings((t) => (Math.Floor(t / 2000) % 2 == 1 ) ? 1 : 0), // Level 5
            VibrationSettings.CreateMixedPatternSettings(
                (VibrationSettings.CreateSinePatternSettings(50), 2000), // sinus i 2 sekunder
                (VibrationSettings.CreateConstantPatternSettings(0.7), 3000) // derefter konstant 70% intensitet i 3 sekunder -> gentag
            ), //level 6
            VibrationSettings.CreateConstantPatternSettings(1), //level 7
        };

        private async Task<int> GetCurrentVibration(IBluetoothPeripheralCharacteristic characteristic)//metode for at gøre navigation lettere
        {
            var currentData = await characteristic.ReadValueAsync(); //læser fra RPI hvad mønsteret er
            var currentPattern = await VibrationSettings.FromBytes(currentData);//læser hvilket level mønster RPI er på

            var currentLevel = 0;
            if (currentPattern != null)
            {
                currentLevel = vibrationsLevels.IndexOf(vibrationsLevels.Find(e => e.Id == currentPattern.Id));
            }
            else
            {

            }
            return currentLevel;
        }
        public async Task<int> NavigateLevelUp()
        {
            if (_patternChar == null)
            {
                return 0;
            }
            var currentVibrationIndex = await GetCurrentVibration(_patternChar);
            var nextLevelIndex = (currentVibrationIndex + 1 + vibrationsLevels.Count) % vibrationsLevels.Count; //finder næste index i vibrations liste
            var nextLevel = vibrationsLevels[nextLevelIndex];//finder næste objekt i vibrations liste
            await _patternChar.WriteValueAsync(nextLevel.ToBytes());//skriver level om til bytes og sender det til RPi
            return nextLevelIndex;
        }
        public async Task<int> NavigateLevelDown()
        {
            if (_patternChar == null)
            {
                return 0;
            }
            var currentVibrationIndex = await GetCurrentVibration(_patternChar);
            var preLevelIndex = (currentVibrationIndex - 1 + vibrationsLevels.Count) % vibrationsLevels.Count; //finder forrige index i vibrations liste
            var preLevel = vibrationsLevels[preLevelIndex];//finder forrige objekt i vibrations liste
            await _patternChar.WriteValueAsync(preLevel.ToBytes());//skriver level om til bytes og sender det til RPi
            return preLevelIndex;
        }
    }
}
