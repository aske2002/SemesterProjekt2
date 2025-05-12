using System.Diagnostics;
using CoreBluetooth;
using Foundation;

namespace tremorur.Models.Bluetooth;
public partial class DiscoveredPeripheral : IDiscoveredPeripheral
{
    public CBPeripheral NativePeripheral { get; private set; }
    private BluetoothService bluetoothService;
    private NSDictionary advertisementData;
    private float rssi = 0;
    public partial float RSSI => rssi;
    public DiscoveredPeripheral(CBDiscoveredPeripheralEventArgs e, BluetoothService service)
    {
        bluetoothService = service;
        NativePeripheral = e.Peripheral;
        advertisementData = e.AdvertisementData;
        rssi = e.RSSI.FloatValue;
        if (advertisementData.ContainsKey(CBAdvertisement.DataTxPowerLevelKey))
        {
            var rssi = advertisementData[CBAdvertisement.DataTxPowerLevelKey] as NSNumber;
            if (rssi != null)
            {

            }
        }
    }
    public partial async Task<IBluetoothPeripheral> ConnectAsync()
    {
        return await bluetoothService.ConnectPeripheralAsync(this);
    }


    public partial bool IsConnectable => advertisementData.TryGetValue(CBAdvertisement.IsConnectable, out var isConnectable) && isConnectable.ToString() == "1";

    public partial List<string> Services
    {
        get
        {
            var services = new List<string>();
            if (advertisementData.ContainsKey(CBAdvertisement.DataServiceUUIDsKey))
            {
                var uuids = advertisementData[CBAdvertisement.DataServiceUUIDsKey];
                if (uuids is NSArray uuidArray)
                {
                    foreach (var uuid in uuidArray)
                    {
                        if (uuid is CBUUID cbUUID)
                        {
                            services.Add(cbUUID.ToString().ToUpper());
                        }
                    }
                }
            }
            return services;
        }
    }
    public partial string? LocalName => advertisementData.TryGetValue(CBAdvertisement.DataLocalNameKey, out var localName) ? localName.ToString() : null;
    public partial string? Name => NativePeripheral.Name;
    public partial string UUID => NativePeripheral.Identifier.AsString().ToUpper();
}