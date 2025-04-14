using System.Collections.Generic;
using System.Linq;

using client.Services.Bluetooth.Gatt.Description;

namespace client.Services.Bluetooth.Gatt.BlueZModel
{
    internal class CharacteristicFlagConverter
    {
        private static readonly Dictionary<CharacteristicFlags, string> FlagMappings =
            new Dictionary<CharacteristicFlags, string>
            {
                {CharacteristicFlags.Read, "read"},
                {CharacteristicFlags.Write, "write"},
                {CharacteristicFlags.WritableAuxiliaries, "writable-auxiliaries"},
                {CharacteristicFlags.Notify, "notify"},
                {CharacteristicFlags.Indicate, "indicate"},
                {CharacteristicFlags.WriteWithoutResponse, "write-without-response"},
            };

        public static string[] ConvertFlags(CharacteristicFlags characteristicFlags)
        {
            return (from mapping in FlagMappings where (characteristicFlags & mapping.Key) > 0 select mapping.Value).ToArray();
        }
    }
}