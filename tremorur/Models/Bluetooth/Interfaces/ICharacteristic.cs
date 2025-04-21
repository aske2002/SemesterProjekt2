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
}
