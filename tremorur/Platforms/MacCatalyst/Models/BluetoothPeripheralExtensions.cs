using System.Diagnostics;
using CoreBluetooth;
using ObjCBindings;

namespace tremorur.Models;

public partial class BluetoothPeripheralCharacteristic
{
    private readonly CBCharacteristic nativeCharacteristic;

    public BluetoothPeripheralCharacteristic(CBCharacteristic cBCharacteristic)
    {
        nativeCharacteristic = cBCharacteristic;
    }

    public partial List<object> Descriptors
    {
        get
        {
            return this.nativeCharacteristic.Descriptors.Select(d => d.Value).Cast<object>().ToList();
        }
    }
    public partial BluetoothCharacteristicProperties Properties
    {
        get
        {

            BluetoothCharacteristicProperties properties = 0;
            if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Broadcast))
                properties |= BluetoothCharacteristicProperties.Broadcast;
            if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Read))
                properties |= BluetoothCharacteristicProperties.Read;
            if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse))
                properties |= BluetoothCharacteristicProperties.WriteWithoutResponse;
            if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Write))
                properties |= BluetoothCharacteristicProperties.Write;
            if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Notify))
                properties |= BluetoothCharacteristicProperties.Notify;
            if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Indicate))
                properties |= BluetoothCharacteristicProperties.Indicate;
            if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.AuthenticatedSignedWrites))
                properties |= BluetoothCharacteristicProperties.AuthenticatedSignedWrites;
            if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.ExtendedProperties))
                properties |= BluetoothCharacteristicProperties.ExtendedProperties;
            if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.NotifyEncryptionRequired))
                properties |= BluetoothCharacteristicProperties.NotifyEncryptionRequired;
            if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.IndicateEncryptionRequired))
                properties |= BluetoothCharacteristicProperties.IndicateEncryptionRequired;
            return properties;
        }
    }
    public partial bool IsNotifying
    {
        get
        {
            return nativeCharacteristic.IsNotifying;
        }
    }
    public partial bool IsBroadcasted
    {
        get
        {
            return nativeCharacteristic.IsBroadcasted;
        }
    }
}
public partial class BluetoothPeripheralService
{
    private readonly CBService nativeService;

    public BluetoothPeripheralService(CBService cBService)
    {
        nativeService = cBService;
    }
    public partial List<BluetoothPeripheralCharacteristic> Characteristics
    {
        get
        {
            return this.nativeService.Characteristics.Select(c => new BluetoothPeripheralCharacteristic(c)).ToList();
        }
    }

    public partial bool IsPrimary
    {
        get
        {
            return this.nativeService.Primary;
        }
    }
}
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