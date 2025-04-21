namespace tremorur.Models.Bluetooth;

public partial class DiscoveredPeripheral : IDiscoveredPeripheral
{
    public partial List<string> Services { get; }
    public partial Guid UUID { get; }
    public partial string? Name { get; }
    public partial string? LocalName { get; }

    public partial float RSSI { get; }
    public partial bool IsConnectable { get; }
    public partial Task<IBluetoothPeripheral> ConnectAsync();
}