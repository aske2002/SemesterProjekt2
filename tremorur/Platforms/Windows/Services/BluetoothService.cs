using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Connectivity;
using Microsoft.Extensions.Logging;
using tremorur.Messages;
using tremorur.Models.Bluetooth;

namespace tremorur.Services;
public partial class BluetoothService
{
    private readonly ILogger<BluetoothService> _logger;
    BluetoothLEHelper bluetoothLEHelper = BluetoothLEHelper.Context;
    public BluetoothService(IMessenger messenger, ILogger<BluetoothService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        //Observable collection
        bluetoothLEHelper.BluetoothLeDevices.CollectionChanged += DiscoveredDevicesChanged;
    }

    public partial bool IsScanning =>
        bluetoothLEHelper.IsEnumerating;

    private void DiscoveredDevicesChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is ObservableBluetoothLEDevice discoveredPeripheral)
                {
                    AddDiscoveredPeripheral(new DiscoveredPeripheral(discoveredPeripheral));
                }
            }
        }
        else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is ObservableBluetoothLEDevice discoveredPeripheral)
                {
                    var peripheral = DiscoveredPeripherals.FirstOrDefault(p => p.NativePeripheral == discoveredPeripheral);
                    if (peripheral != null)
                    {
                        RemoveDiscoveredPeripheral(peripheral);
                    }
                }
            }
        }
    }

    public partial async Task<BluetoothPeripheral> ConnectPeripheralAsync(DiscoveredPeripheral device)
    {
        return await device.ConnectAsync();
    }

    public partial void StartDiscovery()
    {
        if (IsScanning)
        {
            _logger.Log(LogLevel.Information, "Bluetooth scan already in progress.");
            return;
        }
        shouldScan = true;
        bluetoothLEHelper.StartEnumeration();
    }

    public partial void StopDiscovery()
    {
        if (!IsScanning)
        {
            _logger.Log(LogLevel.Information, "Bluetooth scan is not in progress.");
            return;
        }
        shouldScan = false;
        bluetoothLEHelper.StopEnumeration();
    }

}