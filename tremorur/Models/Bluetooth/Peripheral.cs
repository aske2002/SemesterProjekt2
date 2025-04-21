using System.Collections.ObjectModel;
using System.Diagnostics;


namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheral : IBluetoothPeripheral
{
    public partial ObservableCollection<IBluetoothPeripheralService> Services { get; }
    public partial Guid UUID { get; }
    public float? RSSI { get; private set; }
    partial BluetoothPeripheralState State { get; }
    public partial string? Name { get; }
    public partial string? LocalName { get; }
    public partial Task<float?> GetSsriAsync();
}