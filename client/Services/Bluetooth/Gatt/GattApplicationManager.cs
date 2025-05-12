using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Threading.Tasks;
using client.Models;
using client.Services.Bluetooth.Advertisements;
using client.Services.Bluetooth.Core;
using client.Services.Bluetooth.Gatt.BlueZModel;
using client.Services.Bluetooth.Gatt.Description;
using client.Services.Bluetooth.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using shared.Models;
using Tmds.DBus;

namespace client.Services.Bluetooth.Gatt
{
    public record CharacteristicCallback(GattCharacteristic Characteristic, byte[] Value);
    public record AdvertisementConfig(string LocalName, string ServiceUUID, string Type = "peripheral");
    public class GattApplicationManager : IAsyncDisposable
    {
        private readonly ServerContext _serverContext;
        private readonly AdvertisingManager advertisingManager;
        private readonly AdvertisementConfig advertisementConfig;
        public ImmutableList<GattService> Services { get; private set; } = ImmutableList<GattService>.Empty;
        public GattApplicationBuilder Builder { get; } = new GattApplicationBuilder();
        public readonly string AppId = Guid.NewGuid().ToString().Substring(0, 8);
        public string ApplicationObjectPath => $"/{AppId}";
        private GattApplication gattApplication;
        private IMessenger messenger = new WeakReferenceMessenger();
        private readonly ILogger logger = CustomLoggingProvider.CreateLogger<GattApplicationManager>();
        public Dictionary<string[], Func<AsyncRequestProxy<CharChangeData, MessageResponse>, Task<MessageResponse?>>> Handlers { get; } = new Dictionary<string[], Func<AsyncRequestProxy<CharChangeData, MessageResponse>, Task<MessageResponse?>>>();
        public GattApplicationManager(ServerContext serverContext, string localName, string serviceUUID, string type = "peripheral")
        {
            _serverContext = serverContext;
            advertisingManager = new AdvertisingManager(serverContext);
            advertisementConfig = new AdvertisementConfig(localName, serviceUUID, type);
            gattApplication = new GattApplication(ApplicationObjectPath, messenger);
            messenger.Register<GattApplicationManager, CharChangeEvent>(this, CharacteristicDataHandler);
        }
        public async Task<bool> WriteValueAsync(string uuid, byte[] value)
        {
            var characteristic = Services.SelectMany(x => x.Characteristics).FirstOrDefault(x => x.UUID == uuid);
            if (characteristic != null)
            {
                await characteristic.WriteValueAsync(value, new Dictionary<string, object>());
                return true;
            }
            return false;
        }

        public void AddHandler(Func<AsyncRequestProxy<CharChangeData, MessageResponse>, Task<MessageResponse?>> handler, string[]? uuids = null)
        {
            uuids ??= [];
            Handlers.Add(uuids, handler);
        }

        private async void CharacteristicDataHandler(GattApplicationManager _, CharChangeEvent data)
        {
            var handlers = Handlers.Where(x => x.Key.Contains(data.Value.CharacteristicId)).Select(x => x.Value).ToList();
            foreach (var handler in handlers)
            {
                var result = await handler(data.AsProxy());

                if (!data.HasReceivedResponse && result != null)
                {
                    data.Reply(result);
                }
            }

            if (!data.HasReceivedResponse)
            {
                data.Reply(MessageResponse.Success());
            }
        }

        public async Task RunAsync()
        {
            await _serverContext.Connect();
            var adapter = _serverContext.Connection.CreateProxy<IAdapter1>("org.bluez", "/org/bluez/hci0");
            await adapter.SetAliasAsync(advertisementConfig.LocalName);

            var serviceDescriptions = Builder.BuildServiceDescriptions();
            await BuildApplicationTree(serviceDescriptions);
            await RegisterApplicationInBluez(ApplicationObjectPath);

            await advertisingManager.CreateAdvertisement(new AdvertisementProperties()
            {
                LocalName = advertisementConfig.LocalName,
                Type = advertisementConfig.Type,
                ServiceUUIDs = [advertisementConfig.ServiceUUID],
            }, AppId);

        }

        private async Task BuildApplicationTree(IEnumerable<GattServiceDescription> gattServiceDescriptions)
        {
            await _serverContext.Connection.RegisterObjectAsync(gattApplication);

            foreach (var serviceDescription in gattServiceDescriptions)
            {
                var service = await AddNewService(serviceDescription);

                foreach (var characteristicDescription in serviceDescription.GattCharacteristicDescriptions)
                {
                    var characteristic = await AddNewCharacteristic(service, characteristicDescription);

                    foreach (var descriptorDescription in characteristicDescription.Descriptors)
                    {
                        await AddNewDescriptor(characteristic, descriptorDescription);
                    }
                }
            }
        }

        private async Task RegisterApplicationInBluez(string applicationObjectPath)
        {
            var gattManager = _serverContext.Connection.CreateProxy<IGattManager1>("org.bluez", "/org/bluez/hci0");
            await gattManager.RegisterApplicationAsync(new ObjectPath(applicationObjectPath), new Dictionary<string, object>());
        }

        private async Task UnregisterApplicationInBluez(string applicationObjectPath)
        {
            var gattManager = _serverContext.Connection.CreateProxy<IGattManager1>("org.bluez", "/org/bluez/hci0");
            await gattManager.UnregisterApplicationAsync(new ObjectPath(applicationObjectPath));
        }

        private async Task<GattService> AddNewService(
            GattServiceDescription serviceDescription)
        {
            var gattService1Properties = GattPropertiesFactory.CreateGattService(serviceDescription);
            var gattService = gattApplication.AddService(gattService1Properties);
            await _serverContext.Connection.RegisterObjectAsync(gattService);
            Services = Services.Add(gattService);
            return gattService;
        }

        private async Task<GattCharacteristic> AddNewCharacteristic(GattService gattService, GattCharacteristicDescription characteristic)
        {
            var gattCharacteristic1Properties = GattPropertiesFactory.CreateGattCharacteristic(characteristic);
            var gattCharacteristic = gattService.AddCharacteristic(gattCharacteristic1Properties);
            await _serverContext.Connection.RegisterObjectAsync(gattCharacteristic);
            return gattCharacteristic;
        }

        private async Task AddNewDescriptor(GattCharacteristic gattCharacteristic,
            GattDescriptorDescription descriptor)
        {
            var gattDescriptor1Properties = GattPropertiesFactory.CreateGattDescriptor(descriptor);
            var gattDescriptor = gattCharacteristic.AddDescriptor(gattDescriptor1Properties);
            await _serverContext.Connection.RegisterObjectAsync(gattDescriptor);
        }

        public async ValueTask DisposeAsync()
        {
            await advertisingManager.DisposeAsync();
            await UnregisterApplicationInBluez(ApplicationObjectPath);
            foreach (var service in Services)
            {
                await _serverContext.Connection.UnregisterServiceAsync(service.ObjectPath.ToString());
            }
        }
    }
}