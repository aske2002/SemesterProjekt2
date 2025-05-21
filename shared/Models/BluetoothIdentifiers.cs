using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.Models
{
    public static class BluetoothIdentifiers
    {
        // UUIDs for the Advertisement
        public const string AdvertisementType = "peripheral";
        public const string AdvertisementName = "Tremorur";
        // UUIDs for the Vibration Service and its characteristics
        public const string TemorurServiceUUID = "12345678-0000-1000-8000-00805f9b34fb";

        // UUIDs for motor control characteristic Pattern
        public const string VibrationPatternCharacteristicUUID = "12345678-0001-1000-8000-00805f9b34fb";

        // UUIDs for motor status characteristic on/off
        public const string VibrationEnabledCharacteristicUUID = "12345678-0003-1000-8000-00805f9b34fb";

        // Button State Characteristics
        public const string UpButtonStateCharacteristicUUID = "12345678-0004-1000-8000-00805f9b34fb";
        public const string DownButtonStateCharacteristicUUID = "12345678-0005-1000-8000-00805f9b34fb";
        public const string OkButtonStateCharacteristicUUID = "12345678-0006-1000-8000-00805f9b34fb";
        public const string CancelButtonStateCharacteristicUUID = "12345678-0007-1000-8000-00805f9b34fb";

        // Button state characteristics UUID mapping
        public static readonly Dictionary<string, WatchButton> ButtonStateCharacteristicUUIDs = new()
        {
            { UpButtonStateCharacteristicUUID, WatchButton.Up },
            { DownButtonStateCharacteristicUUID, WatchButton.Down },
            { OkButtonStateCharacteristicUUID, WatchButton.Ok },
            { CancelButtonStateCharacteristicUUID, WatchButton.Cancel }
        };
    }
}

