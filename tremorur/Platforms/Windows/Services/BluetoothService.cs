using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Connectivity;
using Microsoft.Extensions.Logging;
using tremorur.Messages;
using tremorur.Models.Bluetooth;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;

namespace tremorur.Services;
public partial class BluetoothService
{
    private readonly string[] RequestedProperties =
            {
                "System.Devices.Aep.Category",
                "System.Devices.Aep.ContainerId",
                "System.Devices.Aep.DeviceAddress",
                "System.Devices.Aep.IsConnected",
                "System.Devices.Aep.IsPaired",
                "System.Devices.Aep.IsPresent",
                "System.Devices.Aep.ProtocolId",
                "System.Devices.Aep.Bluetooth.Le.IsConnectable",
                "System.Devices.Aep.SignalStrength"
            };
    private readonly ILogger<BluetoothService> _logger;
    BluetoothLEAdvertisementWatcher advertisementWatcher;

    public BluetoothService(IMessenger messenger, ILogger<BluetoothService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        //Observable collection

        advertisementWatcher = new BluetoothLEAdvertisementWatcher();
        advertisementWatcher.Received += AdvertisementWatcher_Received;
    }

    public partial bool IsScanning =>
        advertisementWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Started;

    private void AdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        if (ScanForUUIDs != null && ScanForUUIDs.Count() > 0)
        {
            if (ScanForUUIDs.All(uuid => args.Advertisement.ServiceUuids.Any(s => s.ToString().ToUpper() == uuid.ToUpper())))
            {
                AddDiscoveredPeripheral(new DiscoveredPeripheral(args));
            }
        }

    }

    public partial async Task<IBluetoothPeripheral> ConnectPeripheralAsync(IDiscoveredPeripheral device)
    {
        if (device is not DiscoveredPeripheral discoveredPeripheral)
        {
            throw new ArgumentException("Invalid device type", nameof(device));
        }

        return await discoveredPeripheral.ConnectAsync();
    }

    internal partial void startInternalDiscovery()
    {
        if (IsScanning)
        {
            _logger.Log(LogLevel.Information, "Bluetooth scan already in progress.");
            return;
        }
        advertisementWatcher.Start();

        _logger.Log(LogLevel.Information, "Bluetooth scan started.");
    }

    internal partial void stopInternalDiscovery()
    {
        if (!IsScanning)
        {
            _logger.Log(LogLevel.Information, "Bluetooth scan is not in progress.");
            return;
        }
        advertisementWatcher.Stop();
        _logger.Log(LogLevel.Information, "Bluetooth scan stopped.");
    }

}