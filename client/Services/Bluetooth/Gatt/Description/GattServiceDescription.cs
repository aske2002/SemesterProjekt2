using System.Collections.Generic;

namespace client.Services.Bluetooth.Gatt.Description
{
    public class GattServiceDescription
    {
        public IList<GattCharacteristicDescription> GattCharacteristicDescriptions { get; } =
            new List<GattCharacteristicDescription>();

        public required string UUID { get; set; }
        public bool Primary { get; set; }
        public void AddCharacteristic(GattCharacteristicDescription characteristic)
        {
            GattCharacteristicDescriptions.Add(characteristic);
        }
    }
}