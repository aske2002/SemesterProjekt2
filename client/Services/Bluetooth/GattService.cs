using Tmds.DBus;

namespace client.Services.Bluetooth;

class GattService : IGattService1
{
    public ObjectPath ObjectPath { get; } = new ObjectPath("/org/bluez/example/service0");

    public Task<IDictionary<string, object>> GetAllAsync() =>
        Task.FromResult<IDictionary<string, object>>(new Dictionary<string, object>
        {
            { "UUID", "12345678-1234-5678-1234-56789abcde00" },
            { "Primary", true }
        });
}