using System.Collections.ObjectModel;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralService : IBluetoothPeripheralService
{
    public partial string UUID { get; }
    public partial ObservableCollection<IBluetoothPeripheralCharacteristic> Characteristics { get; }
    public event EventHandler<IBluetoothPeripheralCharacteristic> DiscoveredCharacteristic = delegate { };
    public partial bool IsPrimary { get; }
}
