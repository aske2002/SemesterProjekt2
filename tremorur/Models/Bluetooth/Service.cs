using System.Collections.ObjectModel;
using System.Diagnostics;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralService : IBluetoothPeripheralService
{
    public partial string UUID { get; }
    public partial ObservableCollection<IBluetoothPeripheralCharacteristic> Characteristics { get; }
    public partial bool IsPrimary { get; }
}
