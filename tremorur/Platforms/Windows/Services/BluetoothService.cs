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
            if (ScanForUUIDs.All(uuid => args.Advertisement.ServiceUuids.Any(s => s.ToString().ToLower() == uuid.ToLower())))
            {
                AddDiscoveredPeripheral(new DiscoveredPeripheral(args, this));
            }
        }
        else
        {
            AddDiscoveredPeripheral(new DiscoveredPeripheral(args, this));
        }
    }


    private Dictionary<ulong, string> knownDevices
    {
        get
        {
            return SettingsService.GetClassFromStorage<Dictionary<ulong, string>>(nameof(knownDevices), new());
        }
    }
    private void AddKnownDevice(ulong address, string id)
    {
        var devices = knownDevices;
        devices[address] = id;
        SettingsService.SetClassInStorage(nameof(knownDevices), devices);
    }

    internal partial async Task<IBluetoothPeripheral> ConnectPeripheralAsyncInternal(IDiscoveredPeripheral discoveredPeripheral)
    {
        if (discoveredPeripheral is not DiscoveredPeripheral bluetoothDiscoveredPeripheral)
        {
            throw new ArgumentException("Invalid device type", nameof(discoveredPeripheral));
        }
        BluetoothLEDevice nativeDevice;

        if (knownDevices.TryGetValue(bluetoothDiscoveredPeripheral.AdvertisementData.BluetoothAddress, out var id))
        {
            nativeDevice = await BluetoothLEDevice.FromIdAsync(id);
        }
        else
        {
            string aqsFilter = BluetoothLEDevice.GetDeviceSelectorFromBluetoothAddress(bluetoothDiscoveredPeripheral.AdvertisementData.BluetoothAddress);
            var devices = await DeviceInformation.FindAllAsync(aqsFilter);
            var deviceInfo = devices.FirstOrDefault();

            if (deviceInfo == null)
            {
                throw new Exception("Failed to get device information");

            }

            nativeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
            AddKnownDevice(bluetoothDiscoveredPeripheral.AdvertisementData.BluetoothAddress, deviceInfo.Id);
        }

        await ConnectBletoothLEDevice(nativeDevice);
        var peripheral = new BluetoothPeripheral(nativeDevice, bluetoothDiscoveredPeripheral.AdvertisementData, this);
        return peripheral;
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
       var accessResult = await nativeDevice.RequestAccessAsync();

        if (!nativeDevice.DeviceInformation.Pairing.IsPaired)
        {
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
                    throw new Exception($"Failed to pair with device: {pairingResult.Status}");
                }
            }
        }

        if (nativeDevice.ConnectionStatus == BluetoothConnectionStatus.Connected)
        {
            return;
        }

        var retryCount = 0;
        var maxRetries = 20;
        while (nativeDevice.ConnectionStatus != BluetoothConnectionStatus.Connected)
        {
            if (retryCount > maxRetries)
            {
                throw new Exception("Failed to connect to GATT server");
            }

            try
            {
                var result = await nativeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                if (result.Status == GattCommunicationStatus.Success)
                {
                    break;
                }
            }
            catch (Exception) { }
            retryCount++;
            _logger.Log(LogLevel.Warning, $"Failed to connect to GATT server. Retrying {retryCount}/{maxRetries}..."); ;
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