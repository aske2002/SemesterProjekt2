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
        public const string VibrationServiceUUID = "12345678-0000-1000-8000-00805F9B34FB";

        // UUIDs for motor control characteristic Pattern
        public const string VibrationPatternCharacteristicUUID = "12345678-0001-1000-8000-00805F9B34FB";

        // UUIDs for motor status characteristic on/off
        public const string VibrationEnabledCharacteristicUUID = "12345678-0003-1000-8000-00805F9B34FB";

        // UUIDs for the Button Service and its characteristics
        // Button Service
        public const string ButtonServiceUUID = "A1B2C3D4-1000-4000-8000-ABCDEF123456";

        // Button State Characteristics
        public const string UpButtonStateCharacteristicUUID = "A1B2C3D4-1001-4000-8000-ABCDEF123456";
        public const string DownButtonStateCharacteristicUUID = "A1B2C3D4-1002-4000-8000-ABCDEF123456";
        public const string OkButtonStateCharacteristicUUID = "A1B2C3D4-1003-4000-8000-ABCDEF123456";
        public const string CancelButtonStateCharacteristicUUID = "A1B2C3D4-1004-4000-8000-ABCDEF123456";

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

