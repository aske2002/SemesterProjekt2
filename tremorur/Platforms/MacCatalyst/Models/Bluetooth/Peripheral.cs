using System.Collections.ObjectModel;
using System.Diagnostics;
using CoreBluetooth;
using Foundation;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheral : IBluetoothPeripheral
{

    public readonly CBPeripheral NativePeripheral;
    private TaskCompletionSource<float?>? _rssiTaskCompletionSource;
    public BluetoothPeripheral(CBPeripheral cBPeripheral) : base()
    {
        NativePeripheral = cBPeripheral;
        NativePeripheral.DiscoveredService += DiscoveredService;
        NativePeripheral.RssiRead += RssiRead;

        NativePeripheral.DiscoverServices();
        NativePeripheral.ReadRSSI();
    }

    void DiscoveredService(object? sender, NSErrorEventArgs e)
    {
        var allServices = NativePeripheral?.Services?.ToList() ?? new List<CBService>();
        var missingServices = allServices.Where(x => !Services.Any(y => y.UUID == x.UUID.ToString())).ToList();
        foreach (var service in missingServices.Select(x => new BluetoothPeripheralService(x)))
        {
            services.Add(service);
        }
    }

    public void RssiRead(object? peripheral, CBRssiEventArgs e)
    {
        if (e.Error == null)
        {
            _rssiTaskCompletionSource?.TrySetResult(e.Rssi.FloatValue);
            RSSI = e.Rssi.FloatValue;
        }
        else
        {
            _rssiTaskCompletionSource?.TrySetException(new Exception(e.Error?.LocalizedDescription));
            Debug.WriteLine($"Error reading RSSI: {e.Error?.LocalizedDescription}");
        }
    }

    private ObservableCollection<IBluetoothPeripheralService> services = new ObservableCollection<IBluetoothPeripheralService>();
    public partial ObservableCollection<IBluetoothPeripheralService> Services => services;

    public partial Guid UUID => new Guid(NativePeripheral.Identifier.AsString());
    public partial async Task<float?> GetSsriAsync()
    {
        if (_rssiTaskCompletionSource != null)
        {
            Debug.WriteLine("Already reading RSSI");
            return null;
        }
        if (NativePeripheral.State != CBPeripheralState.Connected)
        {
            Debug.WriteLine("Peripheral is not connected");
            return null;
        }
        _rssiTaskCompletionSource = new TaskCompletionSource<float?>();
        NativePeripheral.ReadRSSI();
        if (await Task.WhenAny(_rssiTaskCompletionSource.Task, Task.Delay(1000)) == _rssiTaskCompletionSource.Task)
        {
            return await _rssiTaskCompletionSource.Task;
        }
        else
        {
            Debug.WriteLine("Timeout while reading RSSI");
            return null;
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    partial BluetoothPeripheralState State
    {
        get
        {
            switch (NativePeripheral.State)
            {
                case CBPeripheralState.Connected:
                    return BluetoothPeripheralState.Connected;
                case CBPeripheralState.Disconnected:
                    return BluetoothPeripheralState.Disconnected;
                case CBPeripheralState.Connecting:
                    return BluetoothPeripheralState.Connecting;
                case CBPeripheralState.Disconnecting:
                    return BluetoothPeripheralState.Disconnecting;
                default:
                    return BluetoothPeripheralState.Disconnected;
            }
        }
    }

    public partial string? Name => NativePeripheral.Name;
    public partial string? LocalName => NativePeripheral.Name;
}