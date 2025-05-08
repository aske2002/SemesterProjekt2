namespace tremorur.Models.Bluetooth;

public interface IDiscoveredPeripheral
{
    List<string> Services { get; }
    string UUID { get; }
    string? Name { get; }
    string? LocalName { get; }
    float RSSI { get; }
    bool IsConnectable { get; }
    public Task<IBluetoothPeripheral> ConnectAsync();
}