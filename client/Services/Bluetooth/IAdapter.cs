using Tmds.DBus;
namespace client.Services.Bluetooth;

[DBusInterface("org.bluez.Adapter1")]
interface IAdapter1 : IDBusObject
{
    Task StartDiscoveryAsync();
    Task SetDiscoveryFilterAsync(IDictionary<string, object> Properties);
    Task StopDiscoveryAsync();
    Task RemoveDeviceAsync(ObjectPath Device);
    Task<string[]> GetDiscoveryFiltersAsync();
    Task<T> GetAsync<T>(string prop);
    Task<Adapter1Properties> GetAllAsync();
    Task SetAsync(string prop, object val);
    Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
}

[DBusInterface("org.bluez.Device1")]
interface IDevice1 : IDBusObject
{
    Task DisconnectAsync();
    Task ConnectAsync();
    Task ConnectProfileAsync(string UUID);
    Task DisconnectProfileAsync(string UUID);
    Task PairAsync();
    Task CancelPairingAsync();
    Task<T> GetAsync<T>(string prop);
    Task<Device1Properties> GetAllAsync();
    Task SetAsync(string prop, object val);
    Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
}

[DBusInterface("org.bluez.GattService1")]
interface IGattService1 : IDBusObject
{
    Task<T> GetAsync<T>(string prop);
    Task<GattService1Properties> GetAllAsync();
    Task SetAsync(string prop, object val);
    Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
}

[DBusInterface("org.bluez.GattCharacteristic1")]
interface IGattCharacteristic1 : IDBusObject
{
    Task<byte[]> ReadValueAsync(IDictionary<string, object> Options);
    Task WriteValueAsync(byte[] Value, IDictionary<string, object> Options);
    Task<(CloseSafeHandle fd, ushort mtu)> AcquireWriteAsync(IDictionary<string, object> Options);
    Task<(CloseSafeHandle fd, ushort mtu)> AcquireNotifyAsync(IDictionary<string, object> Options);
    Task StartNotifyAsync();
    Task StopNotifyAsync();
    Task<T> GetAsync<T>(string prop);
    Task<GattCharacteristic1Properties> GetAllAsync();
    Task SetAsync(string prop, object val);
    Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
}