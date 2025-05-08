using System.Diagnostics;

namespace tremorur.Models.Bluetooth;

public interface IBluetoothPeripheralCharacteristic
{
    List<object> Descriptors { get; }
    bool IsNotifying { get; }
    bool IsBroadcasted { get; }
    Task SetNotifyingAsync(bool value);
    Task WriteValueAsync(byte[] data);
    Task<byte[]> ReadValueAsync();
    string UUID { get; }
    BluetoothCharacteristicProperties Properties { get; }
    byte[] LastValue { get; }
    event EventHandler<byte[]> ValueUpdated;
    event EventHandler<bool> NotifyingUpdated;
    string LastValueString => LastValue != null ? System.Text.Encoding.UTF8.GetString(LastValue) : string.Empty;
    bool IsReadable => Properties.HasFlag(BluetoothCharacteristicProperties.Read);
    bool IsWritable => Properties.HasFlag(BluetoothCharacteristicProperties.Write) || Properties.HasFlag(BluetoothCharacteristicProperties.WriteWithoutResponse);
    bool IsNotifiable => Properties.HasFlag(BluetoothCharacteristicProperties.Notify) || Properties.HasFlag(BluetoothCharacteristicProperties.Indicate);
}
