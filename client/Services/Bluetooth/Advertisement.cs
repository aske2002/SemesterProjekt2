using Tmds.DBus;

namespace client.Services.Bluetooth;

public class Advertisement : ILEAdvertisement1
{
    public ObjectPath ObjectPath { get; } = new ObjectPath("/org/bluez/example/advertisement0");

    public Task ReleaseAsync()
    {
        Console.WriteLine("Advertisement released.");
        return Task.CompletedTask;
    }

    public string Type => "peripheral";


    public string[] ServiceUUIDs => new[] { "12345678-1234-5678-1234-56789abcde00" };


    public IDictionary<string, object> ManufacturerData => new Dictionary<string, object>();


    public IDictionary<string, object> ServiceData => new Dictionary<string, object>();


    public string[] Includes => new string[] { };


    public ushort Appearance => 0;


    public string LocalName => "MyBLEPi";


    public ushort Duration => 0;


    public ushort Timeout => 0;
}
