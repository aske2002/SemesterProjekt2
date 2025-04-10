namespace tremorur.Models;

public enum BluetoothPeripheralState
{
    Disconnected = 0,
    Connecting = 1,
    Connected = 2,
    Disconnecting = 3
}

public enum BluetoothCharacteristicProperties : ulong
{
    Broadcast = 1,
    Read = 2,
    WriteWithoutResponse = 4,
    Write = 8,
    Notify = 16,
    Indicate = 32,
    AuthenticatedSignedWrites = 64,
    ExtendedProperties = 128,
    NotifyEncryptionRequired = 256,
    IndicateEncryptionRequired = 512
}

public partial class BluetoothPeripheralCharacteristic
{
    public partial List<object?> Descriptors { get; }
    public partial bool IsNotifying { get; }
    public partial bool IsBroadcasted { get; }
    public partial BluetoothCharacteristicProperties Properties { get; }
}
public partial class BluetoothPeripheralService
{
    public partial List<BluetoothPeripheralCharacteristic> Characteristics { get; }
    public partial bool IsPrimary { get; }
}
public partial class BluetoothPeripheral
{
    public partial List<BluetoothPeripheralService> Services { get; }
    public float? RSSI
    { get; private set; }
    private partial void Initialize();
    partial BluetoothPeripheralState State { get; }
    public string? Name;
    public partial Task<float?> GetSsriAsync();
}