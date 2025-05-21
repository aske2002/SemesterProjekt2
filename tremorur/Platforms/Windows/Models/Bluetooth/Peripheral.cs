using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.WinUI.Connectivity;
using Microsoft.Extensions.Logging;
using shared.Models;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheral
{

    public BluetoothLEDevice NativePeripheral { get; private set; }
    public BluetoothLEAdvertisementReceivedEventArgs AdvertisementData { get; private set; }
    private readonly BluetoothService _bluetoothService;
    private readonly ILogger<BluetoothPeripheral> _logger = CustomLoggingProvider.CreateLogger<BluetoothPeripheral>();
    public BluetoothPeripheral(BluetoothLEDevice leDevice, BluetoothLEAdvertisementReceivedEventArgs advertisement, BluetoothService bluetoothService) : base()
    {
        NativePeripheral = leDevice;
        AdvertisementData = advertisement;
        _bluetoothService = bluetoothService;
        Services_Changed(NativePeripheral, null);
        NativePeripheral.GattServicesChanged += Services_Changed;
        NativePeripheral.ConnectionStatusChanged += ConnectionStatusChanged;
    }

    private void ConnectionStatusChanged(BluetoothLEDevice sender, object args)
    {
        // Handle connection status changes
        if (NativePeripheral.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
        {
            _logger.Log(LogLevel.Information, $"Peripheral {UUID} disconnected");
            _logger.LogWarning($"Peripheral {UUID} disconnected with error {args}");
            _bluetoothService.PeripheralDidDisconnect(UUID, null);
        }
    }

    private void Services_Changed(BluetoothLEDevice sender, object args)
    {
        var allServices = NativePeripheral.GattServices.ToList().OrderBy(x => x.Session.SessionStatus);
        foreach (var service in allServices)
        {
            if (!services.Any(x => x.UUID.ToLower() == service.Uuid.ToString().ToLower()))
            {
                var bluetoothPeripheralService = new BluetoothPeripheralService(service);
                DiscoveredService.Invoke(this, bluetoothPeripheralService);
                services.Add(bluetoothPeripheralService);
            }
        }
    }

    public partial bool IsConnected => NativePeripheral.ConnectionStatus == BluetoothConnectionStatus.Connected;
    public partial string? Name => NativePeripheral.Name;
    public partial string? LocalName => NativePeripheral.DeviceInformation.Name;

    private ObservableCollection<IBluetoothPeripheralService> services = new ObservableCollection<IBluetoothPeripheralService>();
    public partial ObservableCollection<IBluetoothPeripheralService> Services => services;

    public partial string UUID => NativePeripheral.BluetoothDeviceId.ToString().ToLower();
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