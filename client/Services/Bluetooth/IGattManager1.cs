using Tmds.DBus;

namespace client.Services.Bluetooth;

[DBusInterface("org.bluez.GattManager1")]
interface IGattManager1 : IDBusObject
{
    Task RegisterApplicationAsync(ObjectPath application, IDictionary<string, object> options);
    Task UnregisterApplicationAsync(ObjectPath application);
}