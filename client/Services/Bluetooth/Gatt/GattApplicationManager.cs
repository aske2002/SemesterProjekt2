using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Threading.Tasks;
using client.Services.Bluetooth.Advertisements;
using client.Services.Bluetooth.Core;
using client.Services.Bluetooth.Gatt.BlueZModel;
using client.Services.Bluetooth.Gatt.Description;
using Tmds.DBus;

namespace client.Services.Bluetooth.Gatt
{
    public record CharacteristicCallback(string UUID, byte[] Value);
    public record AdvertisementConfig(string LocalName, string Type = "peripheral");
    public class GattApplicationManager
    {
        private readonly ServerContext _serverContext;
        private readonly AdvertisingManager advertisingManager;
        private readonly AdvertisementConfig advertisementConfig;
        public ImmutableList<GattService> Services { get; private set; } = ImmutableList<GattService>.Empty;
        public GattApplicationBuilder Builder { get; } = new GattApplicationBuilder();
        public readonly string AppId = Guid.NewGuid().ToString().Substring(0, 8);
        public string ApplicationObjectPath => $"/{AppId}";
        public Dictionary<string[], Action<CharacteristicCallback>> Handlers { get; } = new Dictionary<string[], Action<CharacteristicCallback>>();
        public GattApplicationManager(ServerContext serverContext, string localName, string type = "peripheral")
        {
            _serverContext = serverContext;
            advertisingManager = new AdvertisingManager(serverContext);
            advertisementConfig = new AdvertisementConfig(localName, type);
        }

        public GattCharacteristic? FirstOrDefault(Expression<Func<GattCharacteristic, bool>> predicate)
        {
            var compiledPredicate = predicate.Compile();
            foreach (var service in Services)
            {
                foreach (var characteristic in service.Characteristics)
                {
                    if (compiledPredicate(characteristic))
                    {
                        return characteristic;
                    }
                }
            }

            return null;
        }

        public void AddHandler(Action<CharacteristicCallback> handler, string[]? uuids = null)
        {
            List<GattCharacteristic> characteristics = Services.SelectMany(x => x.Characteristics).ToList();
            if (uuids != null && uuids.Length > 0)
            {
                characteristics = characteristics.Where(x => uuids.Contains(x.UUID)).ToList();
            }
            Handlers.Add(characteristics.Select(x => x.UUID).ToArray(), handler);
        }

        private void CharacteristicDataHandler(object? sender, byte[] value)
        {
            if (sender is GattCharacteristic characteristic)
            {
                var handlers = Handlers.Where(x => x.Key.Contains(characteristic.UUID)).ToList();
                var args = new CharacteristicCallback(characteristic.UUID, value);
                foreach (var handler in handlers)
                {
                    handler.Value(args);
                }
            }
        }

        public async Task Run()
        {
            await _serverContext.Connect();
            var serviceDescriptions = Builder.BuildServiceDescriptions();
            await advertisingManager.CreateAdvertisement(new AdvertisementProperties()
            {
                LocalName = advertisementConfig.LocalName,
                Type = advertisementConfig.Type,
                ServiceUUIDs = serviceDescriptions.Select(x => x.UUID).ToArray(),
            }, AppId);

            await BuildApplicationTree(ApplicationObjectPath, serviceDescriptions);
            await RegisterApplicationInBluez(ApplicationObjectPath);
            Services.SelectMany(x => x.Characteristics).ToList().ForEach(x =>
            {
                x.OnValueChanged += CharacteristicDataHandler;
            });
        }

        private async Task BuildApplicationTree(string applicationObjectPath, IEnumerable<GattServiceDescription> gattServiceDescriptions)
        {
            var application = await BuildGattApplication(applicationObjectPath);

            foreach (var serviceDescription in gattServiceDescriptions)
            {
                var service = await AddNewService(application, serviceDescription);

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

        private async Task<GattApplication> BuildGattApplication(string applicationObjectPath)
        {
            var application = new GattApplication(applicationObjectPath);
            await _serverContext.Connection.RegisterObjectAsync(application);
            return application;
        }

        private async Task<GattService> AddNewService(GattApplication application,
            GattServiceDescription serviceDescription)
        {
            var gattService1Properties = GattPropertiesFactory.CreateGattService(serviceDescription);
            var gattService = application.AddService(gattService1Properties);
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
    }
}