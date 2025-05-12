using System.Collections.ObjectModel;
using System.Diagnostics;


namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheral : IBluetoothPeripheral
{
    public partial ObservableCollection<IBluetoothPeripheralService> Services { get; }
    public partial string UUID { get; }
    public float? RSSI { get; private set; }
    public partial bool IsConnected { get; }
    partial BluetoothPeripheralState State { get; }
    public partial string? Name { get; }
    public partial string? LocalName { get; }
    public event EventHandler Disconnected = delegate { };
    public partial Task<float?> GetSsriAsync();
}