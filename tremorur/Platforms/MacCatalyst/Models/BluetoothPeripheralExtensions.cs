using System.Diagnostics;
using CoreBluetooth;
using Foundation;

namespace tremorur.Models;

public partial class BluetoothPeripheralCharacteristic
{
    private readonly CBCharacteristic nativeCharacteristic;
    private TaskCompletionSource? writeTaskCompletionSource;
    private TaskCompletionSource<byte[]>? readTaskCompletionSource;
    private CBPeripheral? nativePeripheral => nativeCharacteristic?.Service?.Peripheral;
    private List<Action<byte[]>> notifyActions = new List<Action<byte[]>>();

    public BluetoothPeripheralCharacteristic(CBCharacteristic cBCharacteristic)
    {
        nativeCharacteristic = cBCharacteristic;

        if (nativePeripheral != null)
        {
            nativePeripheral.UpdatedCharacterteristicValue += Characteristic_UpdatedValue;
            nativePeripheral.WroteCharacteristicValue += Characteristic_WroteValue;
        }
    }

    private void Characteristic_WroteValue(object? sender, CBCharacteristicEventArgs e)
    {
        if (e.Characteristic.UUID == nativeCharacteristic.UUID && writeTaskCompletionSource != null)
        {
            if (e.Error == null)
            {
                writeTaskCompletionSource.TrySetResult();
            }
            else
            {
                writeTaskCompletionSource.TrySetException(new Exception(e.Error.LocalizedDescription));
            }
            writeTaskCompletionSource = null;
        }
    }

    private void Characteristic_UpdatedValue(object? sender, CBCharacteristicEventArgs e)
    {
        if (e.Characteristic.UUID != nativeCharacteristic.UUID)
        {
            return;
        }

        var data = e.Characteristic.Value?.ToArray() ?? Array.Empty<byte>();
        foreach (var action in notifyActions)
        {
            action?.Invoke(data);
        }

        if (readTaskCompletionSource != null)
        {
            if (e.Error != null)
            {
                readTaskCompletionSource.TrySetException(new Exception(e.Error.LocalizedDescription));
            }
            else
            {
                readTaskCompletionSource.TrySetResult(data);
            }
        }
    }

    public partial Task NotifyAsync(Action<byte[]> action)
    {
        notifyActions.Add(action);
        return Task.CompletedTask;
    }

    public partial Task StopNotifyAsync(Action<byte[]> action)
    {
        notifyActions.Remove(action);
        return Task.CompletedTask;
    }

    public partial async Task WriteValueAsync(byte[] data)
    {
        if (nativePeripheral == null || nativeCharacteristic == null)
        {
            throw new InvalidOperationException("Peripheral or characteristic is null.");
        }

        var flags = nativeCharacteristic.Properties;
        if (!flags.HasFlag(CBCharacteristicProperties.Write) && !flags.HasFlag(CBCharacteristicProperties.WriteWithoutResponse))
        {
            throw new InvalidOperationException("Characteristic does not support writing.");
        }

        if (writeTaskCompletionSource != null)
        {
            throw new InvalidOperationException("Write operation already in progress.");
        }

        writeTaskCompletionSource = new TaskCompletionSource();
        nativePeripheral.WriteValue(NSData.FromArray(data), nativeCharacteristic, CBCharacteristicWriteType.WithoutResponse);

        if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Write))
        {
            nativePeripheral.WriteValue(NSData.FromArray(data), nativeCharacteristic, CBCharacteristicWriteType.WithResponse);
            await writeTaskCompletionSource.Task;
        }
        else
        {
            nativePeripheral.WriteValue(NSData.FromArray(data), nativeCharacteristic, CBCharacteristicWriteType.WithoutResponse);
        }
    }

    public partial async Task<byte[]> ReadValueAsync()
    {
        if (nativePeripheral == null || nativeCharacteristic == null)
        {
            throw new InvalidOperationException("Peripheral or characteristic is null.");
        }

        if (nativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Read))
        {
            readTaskCompletionSource = new TaskCompletionSource<byte[]>();
            nativePeripheral.ReadValue(nativeCharacteristic);
            return await readTaskCompletionSource.Task;
        }
        else
        {
            throw new InvalidOperationException("Characteristic does not support reading.");
        }
    }

    public partial bool IsNotifying => nativeCharacteristic.IsNotifying;
    public partial bool IsBroadcasted => nativeCharacteristic.IsBroadcasted;
    public partial List<object> Descriptors => this.nativeCharacteristic.Descriptors.Select(d => d.Value).Cast<object>().ToList();
    public partial string UUID => this.nativeCharacteristic.UUID.ToString();
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
    public partial string UUID => this.nativeService.UUID.ToString();
    public partial bool IsPrimary => this.nativeService.Primary;
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