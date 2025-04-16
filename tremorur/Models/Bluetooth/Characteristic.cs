using System.Diagnostics;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralCharacteristic
{
    public partial List<object> Descriptors { get; }
    public partial bool IsNotifying { get; }
    public partial bool IsBroadcasted { get; }
    public partial Task NotifyAsync(Action<byte[]> action);
    public partial Task StopNotifyAsync(Action<byte[]> action);
    public partial Task WriteValueAsync(byte[] data);
    public partial Task<byte[]> ReadValueAsync();
    public partial string UUID { get; }
    public partial BluetoothCharacteristicProperties Properties { get; }
}