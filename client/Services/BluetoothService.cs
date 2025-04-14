using System.Diagnostics;
using client.Services.Bluetooth.Advertisements;
using client.Services.Bluetooth.Core;
using client.Services.Bluetooth.Gatt;
using client.Services.Bluetooth.Gatt.BlueZModel;
using client.Services.Bluetooth.Gatt.Description;
using Microsoft.Extensions.Hosting;

namespace client.Services.Bluetooth
{
    public static class BluetoothIdentifiers
    {
        // UUIDs for the Advertisement
        public const string AdvertisementType = "peripheral";
        public const string AdvertisementName = "Tremor ur";

        // UUIDs for the Vibration Service and its characteristics
        public const string VibrationServiceUUID = "12345678-0000-1000-8000-00805F9B34FB";

        // UUIDs for motor control characteristic
        public const string MotorControlCharacteristicUUID = "12345678-0001-1000-8000-00805F9B34FB";
        public const string MotorControlDescriptorUUID = "12345678-0002-1000-8000-00805F9B34FB";
        public const CharacteristicFlags MotorControlFlags = CharacteristicFlags.Write | CharacteristicFlags.WriteWithoutResponse;

        // UUIDs for motor status characteristic
        public const string MotorStatusCharacteristicUUID = "12345678-0003-1000-8000-00805F9B34FB";
        public const string MotorStatusDescriptorUUID = "12345678-0004-1000-8000-00805F9B34FB";
        public const CharacteristicFlags MotorStatusFlags = CharacteristicFlags.Read | CharacteristicFlags.Notify;

        // UUIDs for motor speed characteristic
        public const string MotorSpeedCharacteristicUUID = "12345678-0005-1000-8000-00805F9B34FB";
        public const string MotorSpeedDescriptorUUID = "12345678-0006-1000-8000-00805F9B34FB";
        public const CharacteristicFlags MotorSpeedFlags = CharacteristicFlags.Read | CharacteristicFlags.Notify;
    }

    public class BluetoothService : IHostedService, IAsyncDisposable
    {
        private readonly List<Tuple<string, string, CharacteristicFlags>> _characteristics = new()
        {
            new Tuple<string, string, CharacteristicFlags>(BluetoothIdentifiers.MotorControlCharacteristicUUID, BluetoothIdentifiers.MotorControlDescriptorUUID, BluetoothIdentifiers.MotorControlFlags),
            new Tuple<string, string, CharacteristicFlags>(BluetoothIdentifiers.MotorStatusCharacteristicUUID, BluetoothIdentifiers.MotorStatusDescriptorUUID, BluetoothIdentifiers.MotorStatusFlags),
            new Tuple<string, string, CharacteristicFlags>(BluetoothIdentifiers.MotorSpeedCharacteristicUUID, BluetoothIdentifiers.MotorSpeedDescriptorUUID, BluetoothIdentifiers.MotorSpeedFlags)
        };
        private readonly ServerContext _serverContext;
        private readonly GattApplicationManager app;
        public BluetoothService()
        {
            _serverContext = new ServerContext();
            app = new GattApplicationManager(_serverContext, BluetoothIdentifiers.AdvertisementName, BluetoothIdentifiers.AdvertisementType);
        }
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        private void registerGattApplication()
        {
            var vibrationServiceDescription = new GattServiceDescription
            {
                UUID = BluetoothIdentifiers.VibrationServiceUUID,
                Primary = true
            };
            var sb = app.Builder.AddService(vibrationServiceDescription);

            foreach (var characteristic in _characteristics)
            {
                var gattCharacteristicDescription = new GattCharacteristicDescription
                {
                    UUID = characteristic.Item1,
                    Flags = characteristic.Item3,
                };

                var gattDescriptorDescription = new GattDescriptorDescription
                {
                    UUID = characteristic.Item2,
                    Value = [0x00],
                    Flags = ["read"]
                };

                sb.WithCharacteristic(gattCharacteristicDescription, [gattDescriptorDescription]);
            }
        }

        public void HandleDataReceived(CharacteristicCallback args)
        {
            switch (args.UUID)
            {
                case BluetoothIdentifiers.MotorControlCharacteristicUUID:
                    // Handle motor control characteristic data
                    break;
                case BluetoothIdentifiers.MotorStatusCharacteristicUUID:
                    // Handle motor status characteristic data
                    break;
                case BluetoothIdentifiers.MotorSpeedCharacteristicUUID:
                    // Handle motor speed characteristic data
                    break;
                default:
                    Debug.WriteLine($"Unknown characteristic UUID: {args.UUID}");
                    break;
            }
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            registerGattApplication();
            app.AddHandler(HandleDataReceived);
            await app.Run();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
