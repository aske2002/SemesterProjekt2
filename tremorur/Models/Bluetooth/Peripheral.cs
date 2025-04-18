using System.Diagnostics;


namespace tremorur.Models.Bluetooth;
public partial class BluetoothPeripheral
{
    public partial List<BluetoothPeripheralService> Services { get; }
    public partial Guid UUID { get; }
    public float? RSSI { get; private set; }
    partial BluetoothPeripheralState State { get; }
    public string? Name;
    public partial Task<float?> GetSsriAsync();
}