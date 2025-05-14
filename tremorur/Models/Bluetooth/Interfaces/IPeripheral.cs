using System.Collections.ObjectModel;
using System.Diagnostics;


namespace tremorur.Models.Bluetooth;

public interface IBluetoothPeripheral
{
    ObservableCollection<IBluetoothPeripheralService> Services { get; }
    public event EventHandler<IBluetoothPeripheralService> DiscoveredService;
    string UUID { get; }
    float? RSSI { get; }
    string? Name { get; }
    string? LocalName { get; }
    bool IsConnected { get; }
    BluetoothPeripheralState State { get; }
    Task<float?> GetSsriAsync();
}