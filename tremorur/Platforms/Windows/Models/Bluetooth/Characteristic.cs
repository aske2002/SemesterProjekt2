using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralCharacteristic
{
    private readonly GattCharacteristic nativeCharacteristic;
    private TaskCompletionSource? notifyTaskCompletionSource;
    private List<Action<byte[]>> notifyActions = new List<Action<byte[]>>();

    public BluetoothPeripheralCharacteristic(GattCharacteristic gattCharacteristic)
    {
        nativeCharacteristic = gattCharacteristic;
        nativeCharacteristic.ValueChanged += Characteristic_UpdatedValue;
    }

    private void Characteristic_UpdatedValue(object? sender, GattValueChangedEventArgs e)
    {
        var data = e.CharacteristicValue.ToArray();
        foreach (var action in notifyActions)
        {
            action?.Invoke(data);
        }
    }

    public partial string UUID => nativeCharacteristic.Uuid.ToString();

    public partial async Task SetNotifyingAsync(bool value)
    {
        var flags = nativeCharacteristic.CharacteristicProperties;
        if (!flags.HasFlag(GattCharacteristicProperties.Notify) && !flags.HasFlag(GattCharacteristicProperties.Indicate))
        {
            throw new InvalidOperationException("Characteristic does not support notifications.");
        }
        var result = await nativeCharacteristic.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(value ? GattClientCharacteristicConfigurationDescriptorValue.Notify : GattClientCharacteristicConfigurationDescriptorValue.None);

        if (result?.Status != null && result.Status != GattCommunicationStatus.Success)
        {
            throw new Exception($"Failed to set notification: {result.Status} {result.ProtocolError}");
        }
    }

    public partial async Task WriteValueAsync(byte[] data)
    {

        var flags = nativeCharacteristic.CharacteristicProperties;
        if (!flags.HasFlag(GattCharacteristicProperties.Write) && !flags.HasFlag(GattCharacteristicProperties.WriteWithoutResponse))
        {
            throw new InvalidOperationException("Characteristic does not support writing.");
        }

        var stream = new InMemoryRandomAccessStream();
        var writer = new DataWriter(stream);
        writer.WriteBytes(data);
        var buffer = writer.DetachBuffer();

        if (nativeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write))
        {
            await nativeCharacteristic.WriteValueWithResultAsync(buffer, GattWriteOption.WriteWithoutResponse);
        }
        else
        {
            await nativeCharacteristic.WriteValueWithResultAsync(buffer, GattWriteOption.WriteWithResponse);
        }
    }

    public partial async Task<byte[]> ReadValueAsync()
    {

        if (!nativeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write))
        {
            throw new InvalidOperationException("Characteristic does not support reading.");

        }

        var response = await nativeCharacteristic.ReadValueAsync();

        if (response.Status != GattCommunicationStatus.Success)
        {
            throw new Exception($"Failed to read characteristic: {response.Status}");
        }
        return response.Value.ToArray();
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