using Tmds.DBus;

namespace client.Services.Bluetooth;

[DBusInterface("org.bluez.GattService1")]
public interface IGattService1 : IDBusObject
{
    Task<IDictionary<string, object>> GetAllAsync();
}