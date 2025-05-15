using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Connectivity;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using tremorur.Messages;
using tremorur.Models.Bluetooth;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace tremorur.Services;
public partial class BluetoothService
{
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
                AddDiscoveredPeripheral(new DiscoveredPeripheral(args, this));
            }
        }
    }

    internal partial async Task<IBluetoothPeripheral> ConnectPeripheralAsyncInternal(IDiscoveredPeripheral discoveredPeripheral)
    {
        if (discoveredPeripheral is not DiscoveredPeripheral bluetoothDiscoveredPeripheral)
        {
            throw new ArgumentException("Invalid device type", nameof(discoveredPeripheral));
        }

        string aqsFilter = BluetoothLEDevice.GetDeviceSelectorFromBluetoothAddress(bluetoothDiscoveredPeripheral.AdvertisementData.BluetoothAddress);
        var devices = await DeviceInformation.FindAllAsync(aqsFilter);
        var deviceInfo = devices.FirstOrDefault();

        if (deviceInfo == null)
        {
            throw new Exception("Failed to get device information");
        }

        var nativeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
        await ConnectBletoothLEDevice(nativeDevice);
     
        var peripheral = new BluetoothPeripheral(nativeDevice, bluetoothDiscoveredPeripheral.AdvertisementData);
        peripheral.ConnectionStatusChanged += BluetoothLEDevice_ConnectionChanged;

        return peripheral;

    }

    private void BluetoothLEDevice_ConnectionChanged(BluetoothPeripheral device, object sender)
    {
        if (device.State == BluetoothPeripheralState.Disconnected || device.State == BluetoothPeripheralState.Disconnecting)
        {
            PeripheralDidDisconnect(device.UUID, string.IsNullOrEmpty(sender.ToString()) ? new Exception(sender.ToString()) : null);
        }
    }

    internal partial async Task ConnectPeripheralAsyncInternal(IBluetoothPeripheral bluetoothPeripheral)
    {
        if (bluetoothPeripheral is not BluetoothPeripheral peripheral)
        {
            throw new ArgumentException("Invalid device type", nameof(bluetoothPeripheral));
        }

        await ConnectBletoothLEDevice(peripheral.NativePeripheral);

    }

    private async Task ConnectBletoothLEDevice(BluetoothLEDevice nativeDevice)
    {
        if (nativeDevice.ConnectionStatus == BluetoothConnectionStatus.Connected) {
            return;
        }

        if (!nativeDevice.DeviceInformation.Pairing.IsPaired)
        {
            var customPairing = nativeDevice.DeviceInformation.Pairing.Custom;
            customPairing.PairingRequested += (sender, args) =>
            {
                args.Accept(); // or use args.Accept(pin) if required
            };
            var pairingResult = await customPairing.PairAsync(DevicePairingKinds.ConfirmOnly, DevicePairingProtectionLevel.Default);

            if (pairingResult.Status != DevicePairingResultStatus.Paired)
            {
                throw new Exception("Failed to pair with device");
            }
        }
        // Get all the services for this device
        var getGattServicesAsyncTokenSource = new CancellationTokenSource(15000);
        var getGattServicesAsyncTask = await
            Task.Run(
                () => nativeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached),
                getGattServicesAsyncTokenSource.Token);

        var result = await getGattServicesAsyncTask;

        if (result.Status != GattCommunicationStatus.Success)
        {
            throw new Exception("Failed to get GATT services");
        }
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