using System.Diagnostics;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralCharacteristic : IBluetoothPeripheralCharacteristic
{
    public partial List<object> Descriptors { get; }
    public partial bool IsNotifying { get; }
    public partial bool IsBroadcasted { get; }
    public partial Task SetNotifyingAsync(bool value);
    public partial Task WriteValueAsync(byte[] data);
    public partial Task<byte[]> ReadValueAsync();
    public partial string UUID { get; }
    public partial BluetoothCharacteristicProperties Properties { get; }
    public partial byte[] LastValue { get; }

    public event EventHandler<byte[]> ValueUpdated = delegate { };
    public event EventHandler<bool> NotifyingUpdated = delegate { };

    public string LastValueString => LastValue != null ? System.Text.Encoding.UTF8.GetString(LastValue) : string.Empty;
    public bool IsReadable => Properties.HasFlag(BluetoothCharacteristicProperties.Read);
    public bool IsWritable => Properties.HasFlag(BluetoothCharacteristicProperties.Write) || Properties.HasFlag(BluetoothCharacteristicProperties.WriteWithoutResponse);
    public bool IsNotifiable => Properties.HasFlag(BluetoothCharacteristicProperties.Notify) || Properties.HasFlag(BluetoothCharacteristicProperties.Indicate);
}