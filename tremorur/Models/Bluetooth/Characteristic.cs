using System.Diagnostics;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralCharacteristic
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
    public string LastValueString => LastValue != null ? System.Text.Encoding.UTF8.GetString(LastValue) : string.Empty;
    public event EventHandler<byte[]> ValueUpdated = delegate { };
}