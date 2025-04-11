using Tmds.DBus;

namespace client.Services.Bluetooth;

[DBusInterface("org.bluez.LEAdvertisement1")]
public interface ILEAdvertisement1 : IDBusObject
{
    Task ReleaseAsync();
}
