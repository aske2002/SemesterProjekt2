using System.Diagnostics;
using CoreBluetooth;
using Foundation;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheral
{

    private readonly CBPeripheral nativePeripheral;
    private TaskCompletionSource<float?>? _rssiTaskCompletionSource;
    public BluetoothPeripheral(CBPeripheral cBPeripheral)
    {
        nativePeripheral = cBPeripheral;
        Name = cBPeripheral.Name;
        Initialize();
    }
    private partial void Initialize()
    {
        nativePeripheral.RssiRead += (sender, e) =>
        {
            if (e.Error == null)
            {
                _rssiTaskCompletionSource?.TrySetResult(e.Rssi.FloatValue);
                _rssiTaskCompletionSource = null;
                RSSI = e.Rssi.FloatValue;
            }
            else
            {
                _rssiTaskCompletionSource?.TrySetException(new Exception(e.Error.LocalizedDescription));
                _rssiTaskCompletionSource = null;
                Debug.WriteLine($"Error reading RSSI: {e.Error}");
            }
        };
    }
    public partial List<BluetoothPeripheralService> Services
    {
        get
        {
            return this.nativePeripheral.Services.Select(s => new BluetoothPeripheralService(s)).ToList();
        }
    }
    public partial async Task<float?> GetSsriAsync()
    {
        if (_rssiTaskCompletionSource != null)
        {
            Debug.WriteLine("Already reading RSSI");
            return null;
        }
        if (nativePeripheral.State != CBPeripheralState.Connected)
        {
            Debug.WriteLine("Peripheral is not connected");
            return null;
        }
        _rssiTaskCompletionSource = new TaskCompletionSource<float?>();
        nativePeripheral.ReadRSSI();
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
    partial BluetoothPeripheralState State
    {
        get
        {
            switch (nativePeripheral.State)
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

}