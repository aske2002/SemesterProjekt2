using Tmds.DBus;

namespace client.Services.Bluetooth;

[DBusInterface("org.bluez.GattApplication1")]
class GattApplication : IDBusObject
{
    public ObjectPath ObjectPath => new ObjectPath("/my/gatt/app");
}

[DBusInterface("org.bluez.GattService1")]
class GattService : IDBusObject
{
    public ObjectPath ObjectPath { get; }
    private readonly IDictionary<string, object> _properties;

    public GattService(string uuid, bool primary, ObjectPath path)
    {
        ObjectPath = path;
        _properties = new Dictionary<string, object>
        {
            { "UUID", uuid },
            { "Primary", primary },
            { "Characteristics", new ObjectPath[] { new ObjectPath($"{path}/char0") } }
        };
    }
    public IDictionary<string, object> GetProperties() => _properties;
}

[DBusInterface("org.bluez.GattCharacteristic1")]
class GattCharacteristic : IDBusObject
{
    public ObjectPath ObjectPath { get; }
    private readonly string _uuid;
    private readonly string[] _flags;

    public GattCharacteristic(string uuid, ObjectPath path, string[] flags)
    {
        ObjectPath = path;
        _uuid = uuid;
        _flags = flags;
    }

    public Task<byte[]> ReadValueAsync(IDictionary<string, object> options)
    {
        var value = System.Text.Encoding.UTF8.GetBytes("Hello BLE");
        return Task.FromResult(value);
    }

    public Task WriteValueAsync(byte[] value, IDictionary<string, object> options)
    {
        Console.WriteLine($"Received write: {System.Text.Encoding.UTF8.GetString(value)}");
        return Task.CompletedTask;
    }

    public IDictionary<string, object> GetProperties() => new Dictionary<string, object>
    {
        { "UUID", _uuid },
        { "Service", new ObjectPath("/my/gatt/app/service0") },
        { "Flags", _flags }
    };
}



[DBusInterface("org.bluez.LEAdvertisement1")]
class LEAdvertisement : IDBusObject
{
    public ObjectPath ObjectPath { get; }
    private readonly IDictionary<string, object> _properties;

    public LEAdvertisement(ObjectPath path, IDictionary<string, object> properties)
    {
        ObjectPath = path;
        _properties = properties;
    }

    public IDictionary<string, object> GetProperties() => _properties;

    public Task ReleaseAsync() => Task.CompletedTask;
}

[DBusInterface("org.bluez.Agent1")]
class NoInputNoOutputAgent : IDBusObject
{
    public ObjectPath ObjectPath => new ObjectPath("/my/agent");

    public Task ReleaseAsync() => Task.CompletedTask;
    public Task RequestPinCodeAsync(ObjectPath device) => Task.FromException(new NotImplementedException());
    public Task DisplayPinCodeAsync(ObjectPath device, string pincode) => Task.CompletedTask;
    public Task RequestPasskeyAsync(ObjectPath device) => Task.FromException(new NotImplementedException());
    public Task DisplayPasskeyAsync(ObjectPath device, uint passkey, ushort entered) => Task.CompletedTask;
    public Task RequestConfirmationAsync(ObjectPath device, uint passkey) => Task.CompletedTask;
    public Task RequestAuthorizationAsync(ObjectPath device) => Task.CompletedTask;
    public Task AuthorizeServiceAsync(ObjectPath device, string uuid) => Task.CompletedTask;
    public Task CancelAsync() => Task.CompletedTask;
}