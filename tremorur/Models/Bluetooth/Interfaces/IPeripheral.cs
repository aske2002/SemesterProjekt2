using System.Collections.ObjectModel;
using System.Diagnostics;


namespace tremorur.Models.Bluetooth;

public interface IBluetoothPeripheral
{
    ObservableCollection<IBluetoothPeripheralService> Services { get; }
    string UUID { get; }
    float? RSSI { get; }
    string? Name { get; }
    event EventHandler Disconnected;
    Task<float?> GetSsriAsync();
}