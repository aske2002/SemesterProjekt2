using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.Models
{
    public static class BluetoothID
    {
        // UUIDs for the Vibration Service and its characteristics
        public const string VibrationServiceUUID = "12345678-0000-1000-8000-00805F9B34FB";

        // UUIDs for motor control characteristic Pattern
        public const string VibrationPatternCharacteristicUUID = "12345678-0001-1000-8000-00805F9B34FB";

        // UUIDs for motor status characteristic on/off
        public const string VibrationEnabledCharacteristicUUID = "12345678-0003-1000-8000-00805F9B34FB";
    }
}

