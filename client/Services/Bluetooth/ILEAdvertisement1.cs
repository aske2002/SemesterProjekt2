using Tmds.DBus;

namespace client.Services.Bluetooth;

[DBusInterface("org.bluez.LEAdvertisement1")]
interface ILEAdvertisement1 : IDBusObject
{
    Task ReleaseAsync();
}
