using client.Services.Bluetooth.Core;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using shared.Models;
using Tmds.DBus;

namespace client.Services.Bluetooth.Gatt.BlueZModel
{
    [DBusInterface("org.freedesktop.DBus.ObjectManager")]
    internal class GattApplication : IObjectManager1
    {
        private readonly IList<GattService> _services = new List<GattService>();
        private readonly IMessenger _messenger;
        private readonly ILogger<GattApplication> _logger = CustomLoggingProvider.CreateLogger<GattApplication>();
        public GattApplication(ObjectPath objectPath, IMessenger messenger)
        {
            _messenger = messenger;
            ObjectPath = objectPath;
            _logger.LogInformation($"GattApplication created: {objectPath}");
        }

        public ObjectPath ObjectPath { get; }

        public Task<IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>> GetManagedObjectsAsync()
        {
            IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>> result =
                new Dictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>();
            foreach (var service in _services)
            {
                result[service.ObjectPath] = service.GetProperties();
                foreach (var characteristic in service.Characteristics)
                {
                    result[characteristic.ObjectPath] = characteristic.GetProperties();
                    foreach (var descriptor in characteristic.Descriptors)
                    {
                        result[descriptor.ObjectPath] = descriptor.GetProperties();
                    }
                }
            }

            return Task.FromResult(result);
        }


        public GattService AddService(GattService1Properties gattService)
        {
            var servicePath = ObjectPath + "/service" + _services.Count;
            var service = new GattService(servicePath, gattService, _messenger);
            _services.Add(service);
            return service;
        }
    }
}