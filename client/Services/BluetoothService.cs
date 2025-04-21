using System.Diagnostics;
using System.Threading.Tasks;
using client.Models;
using client.Services.Bluetooth.Advertisements;
using client.Services.Bluetooth.Core;
using client.Services.Bluetooth.Gatt;
using client.Services.Bluetooth.Gatt.BlueZModel;
using client.Services.Bluetooth.Gatt.Description;
using client.Services.Bluetooth.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using shared.Models.Vibrations;

namespace client.Services.Bluetooth
{
    public static class BluetoothIdentifiers
    {
        // UUIDs for the Advertisement
        public const string AdvertisementType = "peripheral";
        public const string AdvertisementName = "Tremorur";

        // UUIDs for the Vibration Service and its characteristics
        public const string VibrationServiceUUID = "12345678-0000-1000-8000-00805F9B34FB";

        // UUIDs for motor control characteristic Pattern
        public const string VibrationPatternCharacteristicUUID = "12345678-0001-1000-8000-00805F9B34FB";
        public const CharacteristicFlags VibrationPatternFlags = CharacteristicFlags.Write | CharacteristicFlags.WriteWithoutResponse | CharacteristicFlags.Notify | CharacteristicFlags.Read;

        // UUIDs for motor status characteristic on/off
        public const string VibrationEnabledCharacteristicUUID = "12345678-0003-1000-8000-00805F9B34FB";
        public const CharacteristicFlags VibrationEnabledFlags = CharacteristicFlags.Write | CharacteristicFlags.WriteWithoutResponse | CharacteristicFlags.Notify | CharacteristicFlags.Read;
    }

    public class BluetoothService : IHostedService, IRecipient<VibrationsDidToggleEvent>, IRecipient<VibrationSettingsChangedEvent>
    {
        private readonly List<Tuple<string, CharacteristicFlags>> _characteristics = new()
        {
            new Tuple<string, CharacteristicFlags>(BluetoothIdentifiers.VibrationPatternCharacteristicUUID, BluetoothIdentifiers.VibrationPatternFlags),
            new Tuple<string, CharacteristicFlags>(BluetoothIdentifiers.VibrationEnabledCharacteristicUUID, BluetoothIdentifiers.VibrationEnabledFlags),
        };
        private readonly ServerContext serverContext;
        private readonly GattApplicationManager app;
        private readonly ILogger<BluetoothService> logger;
        private readonly IMessenger messenger;
        public BluetoothService(ILogger<BluetoothService> logger, IMessenger messenger)
        {
            this.logger = logger;
            this.messenger = messenger;
            messenger.RegisterAll(this);
            serverContext = new ServerContext();
            app = new GattApplicationManager(serverContext, BluetoothIdentifiers.AdvertisementName, BluetoothIdentifiers.AdvertisementType);
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
                    Flags = characteristic.Item2,
                };

                sb.WithCharacteristic(gattCharacteristicDescription, []);
            }
        }

        public async Task<MessageResponse?> VibrationSettingsSet(AsyncRequestProxy<CharChangeData, MessageResponse> args)
        {
            try
            {
                logger.LogInformation("Vibration settings set");
                logger.LogInformation($"Data: {BitConverter.ToString(args.Value.Data)}");
                
                var settings = await VibrationSettings.FromBytes(args.Value.Data);
                await messenger.Send(new SetVibrationSettingsMessage(settings));
                return MessageResponse.Success();
            }
            catch
            {
                logger.LogError("Failed to parse vibration settings");
                return MessageResponse.Failure("Failed to parse vibration settings");
            }
        }

        public async Task<MessageResponse?> VibrationToggled(AsyncRequestProxy<CharChangeData, MessageResponse> args)
        {
            try
            {
                logger.LogInformation("Vibration toggled");
                logger.LogInformation($"Data: {BitConverter.ToString(args.Value.Data)}");

                var state = args.Value.Data[0] == 1;
                await messenger.Send(new ToggleVibrationsMessage(state));
                return MessageResponse.Success();
            }
            catch
            {
                logger.LogError("Failed to parse vibration state");
                return MessageResponse.Failure("Failed to toggle vibration state");
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            registerGattApplication();

            // Register the GATT characteristic handlers
            app.AddHandler(VibrationSettingsSet, [BluetoothIdentifiers.VibrationPatternCharacteristicUUID]);
            app.AddHandler(VibrationToggled, [BluetoothIdentifiers.VibrationEnabledCharacteristicUUID]);

            // Run the GATT application
            await app.RunAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            serverContext.Dispose();
            messenger.UnregisterAll(this);
            return Task.CompletedTask;
        }

        public async void Receive(VibrationSettingsChangedEvent message)
        {
            await app.WriteValueAsync(BluetoothIdentifiers.VibrationPatternCharacteristicUUID, message.Value.ToBytes());
        }

        public async void Receive(VibrationsDidToggleEvent message)
        {
            await app.WriteValueAsync(BluetoothIdentifiers.VibrationEnabledCharacteristicUUID, [Convert.ToByte(message.Value)]);
        }
    }
}
