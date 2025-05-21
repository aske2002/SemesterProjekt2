using System.Threading.Tasks;
using client.Services.Bluetooth.Core;
using client.Services.Bluetooth.Gatt.BlueZModel;

namespace DotnetBleServer.Gatt
{

    public abstract class ICharacteristicSource
    {
        public PropertiesBase<GattCharacteristic1Properties> Properties;
        public abstract Task WriteValueAsync(byte[] value, bool response);
        public abstract Task<byte[]> ReadValueAsync();
        public abstract Task StartNotifyAsync();
        public abstract Task StopNotifyAsync();
        public abstract Task ConfirmAsync();
    }
}