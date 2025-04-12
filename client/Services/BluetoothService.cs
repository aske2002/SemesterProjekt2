using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DotnetBleServer.Advertisements;
using DotnetBleServer.Core;
using DotnetBleServer.Gatt.Description;
using Microsoft.Extensions.Hosting;
using DotnetBleServer.Gatt;
using System.Reflection.PortableExecutable;
namespace client.Services.Bluetooth
{
    public class BluetoothService : IHostedService, IAsyncDisposable
    {
        private readonly ServerContext _serverContext;
        public BluetoothService()
        {
            _serverContext = new ServerContext();
        }
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        private async Task RegisterAdvertisement()
        {
            var advertisementProperties = new AdvertisementProperties
            {
                Type = "peripheral",
                ServiceUUIDs = ["12345678-1234-5678-1234-56789abcdef0"],
                LocalName = "A",
            };

            await new AdvertisingManager(_serverContext).CreateAdvertisement(advertisementProperties);
        }

        public async Task RegisterGattApplication()
        {
            var gattServiceDescription = new GattServiceDescription
            {
                UUID = "12345678-1234-5678-1234-56789abcdef0",
                Primary = true
            };

            var gattCharacteristicDescription = new GattCharacteristicDescription
            {
                CharacteristicSource = new ExampleCharacteristicSource(),
                UUID = "12345678-1234-5678-1234-56789abcdef1",
                Flags = new[] { "read", "write" },
            };
            var gattDescriptorDescription = new GattDescriptorDescription
            {
                Value = new[] { (byte)'t' },
                UUID = "12345678-1234-5678-1234-56789abcdef2",
                Flags = new[] { "read", "write" }
            };
            var gab = new GattApplicationBuilder();
            gab
                .AddService(gattServiceDescription)
                .WithCharacteristic(gattCharacteristicDescription, new[] { gattDescriptorDescription });

            await new GattApplicationManager(_serverContext).RegisterGattApplication(gab.BuildServiceDescriptions());
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _serverContext.Connect();

            Debug.WriteLine("Connected to Bluetooth stack");
            await RegisterAdvertisement();
            Debug.WriteLine("Advertisement registered");
            await RegisterGattApplication();
            Debug.WriteLine("Gatt application registered");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
