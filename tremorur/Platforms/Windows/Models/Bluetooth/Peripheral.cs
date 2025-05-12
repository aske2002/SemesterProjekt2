using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.WinUI.Connectivity;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheral
{

    public BluetoothLEDevice NativePeripheral { get; private set; }
    public BluetoothLEAdvertisementReceivedEventArgs AdvertisementData { get; private set; }
    public BluetoothPeripheral(BluetoothLEDevice leDevice, BluetoothLEAdvertisementReceivedEventArgs advertisement) : base()
    {
        NativePeripheral = leDevice;
        AdvertisementData = advertisement;
        Services_Changed(NativePeripheral, null);
        NativePeripheral.GattServicesChanged += Services_Changed ;
        NativePeripheral.ConnectionStatusChanged += ConnectionStatusChanged;
    }

    private void ConnectionStatusChanged(BluetoothLEDevice sender, object args)
    {
        // Handle connection status changes
        if (NativePeripheral.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
        {
            Disconnected.Invoke(this, EventArgs.Empty);
        }
    }

    private void Services_Changed(BluetoothLEDevice sender, object args)
    {
        var allServices = NativePeripheral.GattServices.ToList();
        var missingServices = allServices.Where(x => !Services.Any(y => y.UUID == x.Uuid.ToString())).ToList();
        foreach (var service in missingServices.Select(x => new BluetoothPeripheralService(x)))
        {
            services.Add(service);
        }
    }

    public partial bool IsConnected => NativePeripheral.ConnectionStatus == BluetoothConnectionStatus.Connected;
    public partial string? Name => NativePeripheral.Name;
    public partial string? LocalName => NativePeripheral.DeviceInformation.Name;

    private ObservableCollection<IBluetoothPeripheralService> services = new ObservableCollection<IBluetoothPeripheralService>();
    public partial ObservableCollection<IBluetoothPeripheralService> Services => services;

    public partial string UUID => NativePeripheral.BluetoothDeviceId.ToString().ToUpper();
    public partial Task<float?> GetSsriAsync()
    {
        return Task.FromResult((float?)AdvertisementData.RawSignalStrengthInDBm);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public partial BluetoothPeripheralState State
    {
        get
        {
            switch (NativePeripheral.ConnectionStatus)
            {
                case BluetoothConnectionStatus.Connected:
                    return BluetoothPeripheralState.Connected;
                case BluetoothConnectionStatus.Disconnected:
                    return BluetoothPeripheralState.Disconnected;
                default:
                    return BluetoothPeripheralState.Disconnected;
            }
        }
    }
}