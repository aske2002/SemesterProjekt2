using Tmds.DBus;

namespace client.Services.Bluetooth;

class MotorCharacteristic : IGattCharacteristic1
{
    public ObjectPath ObjectPath { get; } = new ObjectPath("/org/bluez/example/service0/char0");
    private byte[] _value = System.Text.Encoding.UTF8.GetBytes("Hello from Pi!");

    public Task<byte[]> ReadValueAsync(IDictionary<string, object> options)
    {
        Console.WriteLine("Characteristic was read");
        return Task.FromResult(_value);
    }

    public Task WriteValueAsync(byte[] value, IDictionary<string, object> options)
    {
        Console.WriteLine("Characteristic was written: " + System.Text.Encoding.UTF8.GetString(value));
        _value = value;
        return Task.CompletedTask;
    }

    public Task<IDictionary<string, object>> GetAllAsync() =>
        Task.FromResult<IDictionary<string, object>>(new Dictionary<string, object>
        {
            { "UUID", "12345678-1234-5678-1234-56789abcdef0" },
            { "Service", new ObjectPath("/org/bluez/example/service0") },
            { "Flags", new string[] { "read", "write" } }
        });
}