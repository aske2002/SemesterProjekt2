using System.Diagnostics;


namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheral : IBluetoothPeripheral
{
    public partial List<IBluetoothPeripheralService> Services { get; }
    public partial Guid UUID { get; }
    public float? RSSI { get; private set; }
    partial BluetoothPeripheralState State { get; }
    public string? Name { get; private set; }
    public partial Task<float?> GetSsriAsync();
}