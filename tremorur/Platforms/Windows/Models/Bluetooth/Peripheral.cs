using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.WinUI.Connectivity;
using Windows.Devices.Bluetooth;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheral
{

    public ObservableBluetoothLEDevice NativePeripheral { get; private set; }
    public BluetoothPeripheral(ObservableBluetoothLEDevice leDevice) : base()
    {
        NativePeripheral = leDevice;
        Initialize();
    }

    private async void Initialize()
    {
        var serviceRes = await NativePeripheral.BluetoothLEDevice.GetGattServicesAsync();
        var allServices = serviceRes.Services.ToList();
        var missingServices = allServices.Where(x => !Services.Any(y => y.UUID == x.Uuid.ToString())).ToList();
        foreach (var service in missingServices.Select(x => new BluetoothPeripheralService(x)))
        {
            services.Add(service);
        }
    }

    public partial string? Name => NativePeripheral.Name;
    public partial string? LocalName => NativePeripheral.DeviceInfo.Name;

    private ObservableCollection<IBluetoothPeripheralService> services = new ObservableCollection<IBluetoothPeripheralService>();
    public partial ObservableCollection<IBluetoothPeripheralService> Services => services;

    public partial Guid UUID => new Guid(NativePeripheral.DeviceInfo.Id);
    public partial Task<float?> GetSsriAsync()
    {
        return Task.FromResult((float?)NativePeripheral.RSSI);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    partial BluetoothPeripheralState State
    {
        get
        {
            switch (NativePeripheral.BluetoothLEDevice.ConnectionStatus)
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