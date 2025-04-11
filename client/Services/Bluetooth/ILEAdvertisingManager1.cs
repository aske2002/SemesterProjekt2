using Tmds.DBus;

namespace client.Services.Bluetooth;

[DBusInterface("org.bluez.LEAdvertisingManager1")]
interface ILEAdvertisingManager1 : IDBusObject
{
    Task RegisterAdvertisementAsync(ObjectPath advertisement, IDictionary<string, object> options);
    Task UnregisterAdvertisementAsync(ObjectPath advertisement);
}
