using System.Diagnostics;
using CommunityToolkit.WinUI.Connectivity;
using Windows.Devices.Bluetooth;

namespace tremorur.Models.Bluetooth;
public partial class DiscoveredPeripheral
{
    public ObservableBluetoothLEDevice NativePeripheral { get; private set; }
    public partial float RSSI => NativePeripheral.RSSI;

    public DiscoveredPeripheral(ObservableBluetoothLEDevice device)
    {
        NativePeripheral = device;
    }
    public partial bool IsConnectable => NativePeripheral.DeviceInfo.Pairing.CanPair;

    public partial List<string> Services => NativePeripheral.Services
        .Select(service => service.UUID)
        .ToList();

    public partial string? LocalName => NativePeripheral.DeviceInfo.Name;
    public partial string? Name => NativePeripheral.Name;
    public partial Guid UUID => new Guid(NativePeripheral.DeviceInfo.Id);
    public partial async Task<IBluetoothPeripheral> ConnectAsync()
    {
        await NativePeripheral.ConnectAsync();
        if (NativePeripheral.IsConnected)
        {
            return new BluetoothPeripheral(NativePeripheral);
        }
        else
        {
            throw new Exception("Failed to connect to peripheral");
        }
    }
}