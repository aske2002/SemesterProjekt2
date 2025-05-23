﻿using client.Services.Bluetooth.Gatt.Description;
using Microsoft.Extensions.Logging;
using shared.Models;
using Tmds.DBus;

namespace client.Services.Bluetooth.Gatt.BlueZModel
{
    internal class GattPropertiesFactory
    {
        public static GattService1Properties CreateGattService(GattServiceDescription serviceDescription)
        {
            CustomLoggingProvider.CreateLogger<GattPropertiesFactory>().LogInformation($"Creating GattService with UUID: {serviceDescription.UUID}");
            return new GattService1Properties
            {
                UUID = serviceDescription.UUID,
                Primary = serviceDescription.Primary,
                Characteristics = new ObjectPath[0]
            };
        }

        public static GattCharacteristic1Properties CreateGattCharacteristic(GattCharacteristicDescription characteristic)
        {
            var characteristicProperties = new GattCharacteristic1Properties { UUID = characteristic.UUID, Flags = CharacteristicFlagConverter.ConvertFlags(characteristic.Flags) };

            return characteristicProperties;
        }

        public static GattDescriptor1Properties CreateGattDescriptor(GattDescriptorDescription descriptor)
        {
            var descriptorProperties = new GattDescriptor1Properties { UUID = descriptor.UUID, Flags = descriptor.Flags, Value = descriptor.Value };

            return descriptorProperties;
        }
    }
}