using System.Diagnostics;
using CoreBluetooth;
using Foundation;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralCharacteristic : IBluetoothPeripheralCharacteristic
{
    private readonly CBCharacteristic nativeCharacteristic;
    private TaskCompletionSource? writeTaskCompletionSource;
    private TaskCompletionSource<byte[]>? readTaskCompletionSource;
    private TaskCompletionSource? notifyTaskCompletionSource;
    private CBPeripheral? nativePeripheral => nativeCharacteristic?.Service?.Peripheral;
    private List<Action<byte[]>> notifyActions = new List<Action<byte[]>>();

    public BluetoothPeripheralCharacteristic(CBCharacteristic cBCharacteristic)
    {
        nativeCharacteristic = cBCharacteristic;

        if (nativePeripheral != null)
        {
            nativePeripheral.UpdatedNotificationState += Peripheral_UpdatedNotificationState;
            nativePeripheral.UpdatedCharacterteristicValue += Characteristic_UpdatedValue;
            nativePeripheral.WroteCharacteristicValue += Characteristic_WroteValue;
        }
    }

    private void Peripheral_UpdatedNotificationState(object? sender, CBCharacteristicEventArgs e)
    {
        if (e.Characteristic.UUID == nativeCharacteristic.UUID)
        {
            if (e.Error == null)
            {
                notifyTaskCompletionSource?.TrySetResult();
            }
            else
            {
                notifyTaskCompletionSource?.TrySetException(new Exception(e.Error.LocalizedDescription));
            }
            notifyTaskCompletionSource = null;
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

    public partial string UUID => nativeCharacteristic.UUID.ToString();

    public partial async Task SetNotifyingAsync(bool value)
    {
        if (nativePeripheral == null || nativeCharacteristic == null)
        {
            throw new InvalidOperationException("Peripheral or characteristic is null.");
        }

        var flags = nativeCharacteristic.Properties;
        if (!flags.HasFlag(CBCharacteristicProperties.Notify) && !flags.HasFlag(CBCharacteristicProperties.Indicate))
        {
            throw new InvalidOperationException("Characteristic does not support notifications.");
        }

        notifyTaskCompletionSource = new TaskCompletionSource();
        nativePeripheral.SetNotifyValue(value, nativeCharacteristic);
        await notifyTaskCompletionSource.Task;
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
    public partial List<object> Descriptors => nativeCharacteristic.Descriptors.Select(d => d.Value).Cast<object>().ToList();
    public partial byte[] LastValue => nativeCharacteristic.Value?.ToArray() ?? Array.Empty<byte>();
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