namespace tremorur.Models.Bluetooth;

public interface IDiscoveredPeripheral
{
    List<string> Services { get; }
    Guid UUID { get; }
    string? Name { get; }
    string? LocalName { get; }
    float RSSI { get; }
    bool IsConnectable { get; }
}