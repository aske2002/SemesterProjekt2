
using System.Text;
using DotnetBleServer.Gatt;

namespace client.Services.Bluetooth;

internal class ExampleCharacteristicSource : ICharacteristicSource
{
    public Task WriteValueAsync(byte[] value)
    {
        Console.WriteLine("Writing value");
        return Task.Run(() => Console.WriteLine(Encoding.ASCII.GetChars(value)));
    }

    public Task<byte[]> ReadValueAsync()
    {
        Console.WriteLine("Reading value");
        return Task.FromResult(Encoding.ASCII.GetBytes("Hello BLE"));
    }
}

[Flags]
public enum CharacteristicFlags
{
    Read = 1,
    Write = 2,
    WritableAuxiliaries = 4
}