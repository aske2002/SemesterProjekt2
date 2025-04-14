using System.Threading.Tasks;

namespace client.Services.Bluetooth.Gatt
{
    public interface ICharacteristicSource
    {
        Task WriteValueAsync(byte[] value);
        Task<byte[]> ReadValueAsync();
    }
}