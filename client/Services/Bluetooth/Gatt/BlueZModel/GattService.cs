using System.Collections.Generic;
using System.Linq;
using client.Services.Bluetooth.Core;
using CommunityToolkit.Mvvm.Messaging;

namespace client.Services.Bluetooth.Gatt.BlueZModel
{
    public class GattService : PropertiesBase<GattService1Properties>, IGattService1, IObjectManagerProperties
    {
        private readonly IList<GattCharacteristic> _characteristics = new List<GattCharacteristic>();
        private readonly IMessenger _messenger;

        public IEnumerable<GattCharacteristic> Characteristics => _characteristics;

        public GattService(string objectPath, GattService1Properties properties, IMessenger messenger) : base(objectPath, properties)
        {
            _messenger = messenger;
        }

        public IDictionary<string, IDictionary<string, object>> GetProperties()
        {
            return new Dictionary<string, IDictionary<string, object>>
            {
                {
                    "org.bluez.GattService1", new Dictionary<string, object>
                    {
                        {"UUID", Properties.UUID},
                        {"Primary", Properties.Primary},
                        {"Characteristics", Characteristics.Select(c => c.ObjectPath).ToArray()}
                    }
                }
            };
        }

        public GattCharacteristic AddCharacteristic(GattCharacteristic1Properties characteristic)
        {
            characteristic.Service = ObjectPath;
            var gattCharacteristic = new GattCharacteristic(NextCharacteristicPath(), characteristic, _messenger);
            _characteristics.Add(gattCharacteristic);

            Properties.Characteristics = Properties.Characteristics.Append(NextCharacteristicPath()).ToArray();

            return gattCharacteristic;
        }

        private string NextCharacteristicPath()
        {
            return ObjectPath + "/characteristic" + _characteristics.Count;
        }
    }
}