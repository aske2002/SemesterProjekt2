using System.Diagnostics;
using System.Text;
using client.Services.Bluetooth.Core;
using client.Services.Bluetooth.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using shared.Models;
using Tmds.DBus;

namespace client.Services.Bluetooth.Gatt.BlueZModel
{
    public class GattCharacteristic : PropertiesBase<GattCharacteristic1Properties>, IGattCharacteristic1
    {
        public string UUID => Properties.UUID;
        private ILogger<GattCharacteristic> _logger { get; } = CustomLoggingProvider.CreateLogger<GattCharacteristic>();
        private readonly IMessenger _messenger;
        public IList<GattDescriptor> Descriptors { get; } = new List<GattDescriptor>();
        public GattCharacteristic(ObjectPath objectPath, GattCharacteristic1Properties properties, IMessenger messenger) : base(objectPath, properties)
        {
            _messenger = messenger;
        }

        public Task<byte[]> ReadValueAsync(IDictionary<string, object> options)
        {
            _logger.LogInformation($"Reading value for characteristic {Properties.UUID}");
            return Task.FromResult(Properties.Value);
        }

        // public Task ConfirmAsync()
        // {
        //     _logger.LogInformation($"Confirming notification for characteristic {Properties.UUID}");
        //     return Task.CompletedTask;
        // }

        public async Task WriteValueAsync(byte[] value, IDictionary<string, object> options)
        {

            _logger.LogInformation($"Writing value for characteristic {Properties.UUID}");
            foreach (var option in options)
            {
                _logger.LogInformation($"Option: {option.Key} = {option.Value}");
            }

            var response = await _messenger.Send(new CharChangeEvent(new CharChangeData
            {
                DeviceId = options.TryGetValue("device", out var deviceId) ? deviceId.ToString() : null,
                CharacteristicId = Properties.UUID,
                Data = value
            }));
            if (response != null && response.Error)
            {
                await SetAsync("Value", value);
            }
            else
            {
                throw new DBusException("org.bluez.Error.Failed", response?.Message);
            }
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


        public async Task StartNotifyAsync()
        {
            _logger.LogInformation($"Starting notification for characteristic {Properties.UUID}");
            await SetAsync("Notifying", true);
        }

        public async Task StopNotifyAsync()
        {
            _logger.LogInformation($"Stopping notification for characteristic {Properties.UUID}");
            await SetAsync("Notifying", false);
        }
    }
}