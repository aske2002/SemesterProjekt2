using client.Models;
using client.Services.Bluetooth.Core;
using client.Services.Bluetooth.Gatt;
using client.Services.Bluetooth.Gatt.Description;
using client.Services.Bluetooth.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using shared.Messages;
using shared.Models;
using shared.Models.Vibrations;

namespace client.Services.Bluetooth
{
    public static class BluetoothFlags
    {
        public const CharacteristicFlags VibrationPatternFlags = CharacteristicFlags.Write | CharacteristicFlags.WriteWithoutResponse | CharacteristicFlags.Notify | CharacteristicFlags.Read;
        public const CharacteristicFlags VibrationEnabledFlags = CharacteristicFlags.Write | CharacteristicFlags.WriteWithoutResponse | CharacteristicFlags.Notify | CharacteristicFlags.Read;
        public const CharacteristicFlags ButtonStateFlags = CharacteristicFlags.Notify | CharacteristicFlags.Read;
    }

    public class BluetoothService : IHostedService, IRecipient<VibrationsDidToggleEvent>, IRecipient<VibrationSettingsChangedEvent>, IRecipient<ButtonStateChangedMessage>
    {
        private readonly Dictionary<string, Dictionary<string, CharacteristicFlags>> _services = new()
        {
            {BluetoothIdentifiers.ButtonServiceUUID, BluetoothIdentifiers.ButtonStateCharacteristicUUIDs.ToDictionary(x => x.Key, x => BluetoothFlags.ButtonStateFlags)},
            {BluetoothIdentifiers.VibrationServiceUUID, new Dictionary<string, CharacteristicFlags>
                {
                    {BluetoothIdentifiers.VibrationPatternCharacteristicUUID, BluetoothFlags.VibrationPatternFlags},
                    {BluetoothIdentifiers.VibrationEnabledCharacteristicUUID, BluetoothFlags.VibrationEnabledFlags}
                }
            }
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
            app = new GattApplicationManager(serverContext, BluetoothIdentifiers.AdvertisementName, BluetoothIdentifiers.VibrationServiceUUID, BluetoothIdentifiers.AdvertisementType);
        }

        private void registerGattApplication()
        {
            for (var i = 0; i < _services.Count; i++)
            {
                var service = _services.ElementAt(i);
                logger.LogInformation($"Registering service {i + 1}/{_services.Count}: {service.Key}");
                var serviceDescription = new GattServiceDescription
                {
                    UUID = service.Key,
                    Primary = true,
                };
                var sb = app.Builder.AddService(serviceDescription);
                foreach (var characteristic in service.Value)
                {
                    logger.LogInformation($" - Registering characteristic {characteristic.Key} with flags {characteristic.Value}");
                    var gattCharacteristicDescription = new GattCharacteristicDescription
                    {
                        UUID = characteristic.Key,
                        Flags = characteristic.Value,
                    };

                    sb.WithCharacteristic(gattCharacteristicDescription, []);
                }
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to parse vibration settings");
                logger.LogError($"Data: {BitConverter.ToString(args.Value.Data)}");
                logger.LogError($"Exception: {ex.Message}");
                logger.LogError($"StackTrace: {ex.StackTrace}");
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
            logger.LogInformation("Starting bluetooth service");
            registerGattApplication();

            // Register the GATT characteristic handlers
            app.AddHandler(VibrationSettingsSet, [BluetoothIdentifiers.VibrationPatternCharacteristicUUID]);
            app.AddHandler(VibrationToggled, [BluetoothIdentifiers.VibrationEnabledCharacteristicUUID]);

            // Run the GATT application
            await app.RunAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await app.DisposeAsync();
            serverContext.Dispose();
            messenger.UnregisterAll(this);
        }

        public async void Receive(VibrationSettingsChangedEvent message)
        {
            await app.WriteValueAsync(BluetoothIdentifiers.VibrationPatternCharacteristicUUID, message.Value.ToBytes());
        }

        public async void Receive(VibrationsDidToggleEvent message)
        {
            await app.WriteValueAsync(BluetoothIdentifiers.VibrationEnabledCharacteristicUUID, [Convert.ToByte(message.Value)]);
        }

        public async void Receive(ButtonStateChangedMessage message)
        {
            var uuid = BluetoothIdentifiers.ButtonStateCharacteristicUUIDs.FirstOrDefault(x => x.Value == message.Value.Button).Key;
            if (uuid == null)
            {
                logger.LogError("Failed to parse button state");
                return;
            }
            await app.WriteValueAsync(uuid, message.Value.ToBytes());
        }
    }
}
