using Tmds.DBus;

namespace client.Services.Bluetooth;

[DBusInterface("org.bluez.GattCharacteristic1")]
interface IGattCharacteristic1 : IDBusObject
{
    Task<byte[]> ReadValueAsync(IDictionary<string, object> options);
    Task WriteValueAsync(byte[] value, IDictionary<string, object> options);
    Task<IDictionary<string, object>> GetAllAsync();
}