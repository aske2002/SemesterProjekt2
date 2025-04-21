using System.Diagnostics;


namespace tremorur.Models.Bluetooth;

public interface IBluetoothPeripheral
{
    List<IBluetoothPeripheralService> Services { get; }
    Guid UUID { get; }
    float? RSSI { get; }
    string? Name { get; }
    Task<float?> GetSsriAsync();
}