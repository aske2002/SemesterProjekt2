using client.Services.Bluetooth.Core;
using Tmds.DBus;

namespace client.Services.Bluetooth.Gatt.BlueZModel
{
    public class GattCharacteristic : PropertiesBase<GattCharacteristic1Properties>, IGattCharacteristic1,
        IObjectManagerProperties
    {
        private byte[] _value = Array.Empty<byte>();
        public byte[] Value
        {
            get => _value; set
            {
                _value = value;
                OnValueChanged(this, value);
            }
        }
        public string UUID => Properties.UUID;

        public event EventHandler<byte[]> OnValueChanged = delegate { };
        public IList<GattDescriptor> Descriptors { get; } = new List<GattDescriptor>();

        public GattCharacteristic(ObjectPath objectPath, GattCharacteristic1Properties properties) : base(objectPath, properties)
        {
        }

        public Task<byte[]> ReadValueAsync(IDictionary<string, object> options)
        {
            return Task.FromResult(Value);
        }

        public Task WriteValueAsync(byte[] value, IDictionary<string, object> options)
        {
            return Task.FromResult(Value = value);
        }

        public Task StartNotifyAsync()
        {
            throw new NotImplementedException();
        }

        public Task StopNotifyAsync()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IDictionary<string, object>> GetProperties()
        {
            return new Dictionary<string, IDictionary<string, object>>
            {
                {
                    "org.bluez.GattCharacteristic1", new Dictionary<string, object>
                    {
                        {"Service", Properties.Service},
                        {"UUID", Properties.UUID},
                        {"Flags", Properties.Flags},
                        {"Descriptors", Descriptors.Select(d => d.ObjectPath).ToArray()}
                    }
                }
            };
        }

        public GattDescriptor AddDescriptor(GattDescriptor1Properties gattDescriptorProperties)
        {
            gattDescriptorProperties.Characteristic = ObjectPath;
            var gattDescriptor = new GattDescriptor(NextDescriptorPath(), gattDescriptorProperties);
            Descriptors.Add(gattDescriptor);
            return gattDescriptor;
        }

        private ObjectPath NextDescriptorPath()
        {
            return ObjectPath + "/descriptor" + Descriptors.Count;
        }
    }
}