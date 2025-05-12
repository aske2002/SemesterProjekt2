using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Extensions.Logging;
using shared.Models;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralCharacteristic
{
    private readonly GattCharacteristic nativeCharacteristic;
    private TaskCompletionSource? notifyTaskCompletionSource;
    private List<Action<byte[]>> notifyActions = new List<Action<byte[]>>();
    private bool isNotifying = false;
    private bool isBroadcasted = false;
    private byte[] lastValue = Array.Empty<byte>();

    public BluetoothPeripheralCharacteristic(GattCharacteristic gattCharacteristic)
    {
        nativeCharacteristic = gattCharacteristic;
        nativeCharacteristic.ValueChanged += Characteristic_UpdatedValue;
    }

    private async Task readDescriptors()
    {
        var result = await nativeCharacteristic.ReadClientCharacteristicConfigurationDescriptorAsync();
        if (result.ClientCharacteristicConfigurationDescriptor.HasFlag(GattClientCharacteristicConfigurationDescriptorValue.Notify))
        {
            isNotifying = true;
        }
        else
        {
            isNotifying = false;
        }

        if (result.ClientCharacteristicConfigurationDescriptor.HasFlag(GattClientCharacteristicConfigurationDescriptorValue.Indicate))
        {
            isBroadcasted = true;
        }
        else
        {
            isBroadcasted = false;
        }
    }

    private void Characteristic_UpdatedValue(object? sender, GattValueChangedEventArgs e)
    {
        var data = e.CharacteristicValue.ToArray();
        
        if (data.Length == 0)
        {
            return;
        }

        foreach (var action in notifyActions)
        {
            action?.Invoke(data);
        }
        ValueUpdated.Invoke(this, data);
    }

    public partial string UUID => nativeCharacteristic.Uuid.ToString().ToUpper();

    public partial async Task SetNotifyingAsync(bool value)
    {
        var flags = nativeCharacteristic.CharacteristicProperties;
        if (!flags.HasFlag(GattCharacteristicProperties.Notify) && !flags.HasFlag(GattCharacteristicProperties.Indicate))
        {
            throw new InvalidOperationException("Characteristic does not support notifications.");
        }
        var result = await nativeCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(value ? GattClientCharacteristicConfigurationDescriptorValue.Notify : GattClientCharacteristicConfigurationDescriptorValue.None);


        if (result != GattCommunicationStatus.Success)
        {
            throw new Exception($"Failed to set notification: {result}");
        }

        await readDescriptors();
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

    public partial bool IsNotifying => isNotifying;
    public partial bool IsBroadcasted => isBroadcasted;
    public partial List<object> Descriptors => nativeCharacteristic.GetAllDescriptors().Cast<object>().ToList();

    public partial byte[] LastValue => lastValue;
    public partial BluetoothCharacteristicProperties Properties
    {
        get
        {

            BluetoothCharacteristicProperties properties = 0;
            if (nativeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Broadcast))
                properties |= BluetoothCharacteristicProperties.Broadcast;
            if (nativeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                properties |= BluetoothCharacteristicProperties.Read;
            if (nativeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse))
                properties |= BluetoothCharacteristicProperties.WriteWithoutResponse;
            if (nativeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write))
                properties |= BluetoothCharacteristicProperties.Write;
            if (nativeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                properties |= BluetoothCharacteristicProperties.Notify;
            if (nativeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                properties |= BluetoothCharacteristicProperties.Indicate;
            if (nativeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.AuthenticatedSignedWrites))
                properties |= BluetoothCharacteristicProperties.AuthenticatedSignedWrites;
            if (nativeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.ExtendedProperties))
                properties |= BluetoothCharacteristicProperties.ExtendedProperties;
            return properties;
        }
    }
}