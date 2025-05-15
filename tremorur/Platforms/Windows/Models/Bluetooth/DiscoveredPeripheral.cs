using System.Diagnostics;
using CommunityToolkit.WinUI.Connectivity;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace tremorur.Models.Bluetooth;
public partial class DiscoveredPeripheral
{
    public BluetoothLEAdvertisementReceivedEventArgs AdvertisementData { get; private set; }
    public partial float RSSI => AdvertisementData.RawSignalStrengthInDBm;
    private readonly BluetoothService _bluetoothService;

    public DiscoveredPeripheral(BluetoothLEAdvertisementReceivedEventArgs advertisementData, BluetoothService bluetoothService)
    {
        AdvertisementData = advertisementData;
        _bluetoothService = bluetoothService;
    }
    public partial bool IsConnectable => AdvertisementData.IsConnectable;

    public partial List<string> Services => AdvertisementData.Advertisement.ServiceUuids
        .Select(uuid => uuid.ToString().ToUpper())
        .ToList();

    public partial string? LocalName => AdvertisementData.Advertisement.LocalName;
    public partial string? Name => AdvertisementData.BluetoothAddress.ToString();
    public partial string UUID => AdvertisementData.BluetoothAddress.ToString().ToUpper();
    public partial async Task<IBluetoothPeripheral> ConnectAsync()
    {
        return await _bluetoothService.ConnectPeripheralAsync(this);

    }
}