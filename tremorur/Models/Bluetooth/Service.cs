using System.Diagnostics;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralService : IBluetoothPeripheralService
{
    public partial string UUID { get; }
    public partial List<IBluetoothPeripheralCharacteristic> Characteristics { get; }
    public partial bool IsPrimary { get; }
}
