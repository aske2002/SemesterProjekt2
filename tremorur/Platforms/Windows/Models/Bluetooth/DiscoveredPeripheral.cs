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

    public DiscoveredPeripheral(BluetoothLEAdvertisementReceivedEventArgs advertisementData)
    {
        AdvertisementData = advertisementData;
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
        string aqsFilter = BluetoothLEDevice.GetDeviceSelectorFromBluetoothAddress(AdvertisementData.BluetoothAddress);
        var devices = await DeviceInformation.FindAllAsync(aqsFilter);
        var deviceInfo = devices.FirstOrDefault();

        if (deviceInfo == null)
        {
            throw new Exception("Failed to get device information");
        }

        var nativeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);


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

        var peripheral = new BluetoothPeripheral(nativeDevice, AdvertisementData);
        return peripheral;

    }
}