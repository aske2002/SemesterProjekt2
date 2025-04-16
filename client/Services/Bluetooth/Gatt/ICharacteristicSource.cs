using System.Threading.Tasks;

namespace client.Services.Bluetooth.Gatt
{
    public interface ICharacteristicSource
    {
        Task WriteValueAsync(byte[] value);
        Task<byte[]> ReadValueAsync();
    }
    
    public class DefaultCharacteristicSource : ICharacteristicSource
    {
        public Task WriteValueAsync(byte[] value)
        {
            var str = System.Text.Encoding.UTF8.GetString(value);
            Console.WriteLine($"WriteValueAsync: {str}");
            return Task.CompletedTask;
        }

        public Task<byte[]> ReadValueAsync()
        {
            return Task.FromResult(Array.Empty<byte>());
        }
    }
}