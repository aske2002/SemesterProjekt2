using System.Diagnostics;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralService
{
    public partial string UUID { get; }
    public partial List<BluetoothPeripheralCharacteristic> Characteristics { get; }
    public partial bool IsPrimary { get; }
}
