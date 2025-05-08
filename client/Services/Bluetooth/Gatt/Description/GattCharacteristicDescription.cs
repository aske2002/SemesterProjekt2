using System.Collections.Generic;

namespace client.Services.Bluetooth.Gatt.Description
{
    public class GattCharacteristicDescription
    {
        private readonly IList<GattDescriptorDescription> _Descriptors = new List<GattDescriptorDescription>();

        public IEnumerable<GattDescriptorDescription> Descriptors => _Descriptors;
        public required string UUID { get; set; }
        public CharacteristicFlags Flags { get; set; }

        public void AddDescriptor(GattDescriptorDescription gattDescriptorDescription)
        {
            _Descriptors.Add(gattDescriptorDescription);
        }
    }
}