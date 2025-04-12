using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tmds.DBus;

namespace client.Services.Bluetooth
{
    public class BluetoothService : IHostedService, IAsyncDisposable
    {
        private readonly Connection _connection;
        private IObjectManager _objectManager;
        private IAdapter1 _adapter;
        private IDisposable _interfaceAddedWatcher;
        private IDisposable _interfaceRemovedWatcher;

        public BluetoothService()
        {
            _connection = new Connection(Address.System);
        }

        public async Task EnableBleAdvertisingAsync()
        {
            await _adapter.SetPoweredAsync(true);
            await _adapter.SetDiscoverableAsync(true);

            var advManager = _connection.CreateProxy<ILEAdvertisingManager1>("org.bluez", "/org/bluez/hci0");

            var advertisementPath = new ObjectPath("/my/advertisement");

            var adv = new LEAdvertisement(advertisementPath, new Dictionary<string, object>
            {
                { "Type", "peripheral" },
                { "ServiceUUIDs", new string[] { "12345678-1234-5678-1234-56789abcdef0" } },
                { "LocalName", "MyBLEDevice" }
            });

            await _connection.RegisterObjectAsync(adv);

            await advManager.RegisterAdvertisementAsync(advertisementPath, new Dictionary<string, object>());
        }

        public async Task RegisterSerialProfileAsync()
        {
            var profileManager = _connection.CreateProxy<IProfileManager1>("org.bluez", "/org/bluez");

            var options = new Dictionary<string, object>
    {
        { "Name", "My Serial Port" },
        { "Role", "server" },
        { "RequireAuthentication", true },
        { "RequireAuthorization", false },
        { "Channel", (byte)22 }, // Make sure this doesn't conflict
        { "Service", "00001101-0000-1000-8000-00805f9b34fb" } // SPP UUID
    };

            await profileManager.RegisterProfileAsync(
                new ObjectPath("/my/serial/profile"),
                "00001101-0000-1000-8000-00805f9b34fb",
                options
            );
        }

        public async Task RegisterGattServerAsync()
        {
            var gattApp = new GattApplication();
            var gattService = new GattService("12345678-1234-5678-1234-56789abcdef0", true, new ObjectPath("/my/gatt/app/service0"));
            var gattChar = new GattCharacteristic("12345678-1234-5678-1234-56789abcdef1", new ObjectPath("/my/gatt/app/service0/char0"), new[] { "read", "write" });

            await _connection.RegisterObjectAsync(gattApp);
            await _connection.RegisterObjectAsync(gattService);
            await _connection.RegisterObjectAsync(gattChar);

            var gattManager = _connection.CreateProxy<IGattManager1>("org.bluez", "/org/bluez/hci0");

            await gattManager.RegisterApplicationAsync(
                new ObjectPath("/my/gatt/app"),
                new Dictionary<string, object>() // no options needed for now
            );
        }

        public async Task RegisterAgentAsync()
        {
            var agent = new NoInputNoOutputAgent();
            await _connection.RegisterObjectAsync(agent);

            var agentManager = _connection.CreateProxy<IAgentManager1>("org.bluez", "/org/bluez");
            await agentManager.RegisterAgentAsync(agent.ObjectPath, "NoInputNoOutput");
            await agentManager.RequestDefaultAgentAsync(agent.ObjectPath);
        }


        public async Task StartDiscoveryAsync() => await _adapter.StartDiscoveryAsync();
        public async Task StopDiscoveryAsync() => await _adapter.StopDiscoveryAsync();

        public async Task<bool> IsDiscoveringAsync() => await _adapter.GetDiscoveringAsync();
        public async Task<string> GetAdapterAddressAsync() => await _adapter.GetAddressAsync();

        public async Task<IDisposable> WatchInterfacesAddedAsync(Action<(ObjectPath, IDictionary<string, IDictionary<string, object>>)> handler)
        {
            _interfaceAddedWatcher = await _objectManager.WatchInterfacesAddedAsync(handler, ex => Debug.WriteLine($"Error: {ex.Message}"));
            return _interfaceAddedWatcher;
        }

        public async Task<IDisposable> WatchInterfacesRemovedAsync(Action<(ObjectPath, string[])> handler)
        {
            _interfaceRemovedWatcher = await _objectManager.WatchInterfacesRemovedAsync(handler, ex => Debug.WriteLine($"Error: {ex.Message}"));
            return _interfaceRemovedWatcher;
        }

        public async Task SetDiscoverableAsync(bool enable) => await _adapter.SetDiscoverableAsync(enable);
        public async Task SetPoweredAsync(bool enable) => await _adapter.SetPoweredAsync(enable);
        public async Task SetPairableAsync(bool enable) => await _adapter.SetPairableAsync(enable);
        public async Task<bool> IsDiscoverableAsync() => await _adapter.GetDiscoverableAsync();
        public async Task<bool> IsPoweredAsync() => await _adapter.GetPoweredAsync();
        public async Task<bool> IsPairableAsync() => await _adapter.GetPairableAsync();
        public async Task<string> GetAdapterNameAsync() => await _adapter.GetNameAsync();
        public async Task SetAliasAsync(string name) => await _adapter.SetAliasAsync(name);
        public async Task<string> GetAliasAsync() => await _adapter.GetAliasAsync();
        public async Task SetDiscoverableTimeoutAsync(uint timeout) => await _adapter.SetDiscoverableTimeoutAsync(timeout);

        public async Task EnableIncomingConnectionsAsync()
        {
            await _adapter.SetPoweredAsync(true);
            await _adapter.SetPairableAsync(true);
            await _adapter.SetDiscoverableAsync(true);
            await _adapter.SetDiscoverableTimeoutAsync(0); // 0 = forever
        }
        public async ValueTask DisposeAsync()
        {
            _interfaceAddedWatcher?.Dispose();
            _interfaceRemovedWatcher?.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            await _connection.ConnectAsync();

            _objectManager = _connection.CreateProxy<IObjectManager>("org.bluez", "/");

            var managedObjects = await _objectManager.GetManagedObjectsAsync();

            foreach (var obj in managedObjects)
            {
                if (obj.Value.ContainsKey("org.bluez.Adapter1"))
                {
                    _adapter = _connection.CreateProxy<IAdapter1>("org.bluez", obj.Key);
                    break;
                }
            }

            if (_adapter == null)
                throw new Exception("No Bluetooth adapter found.");

            Console.WriteLine("Starting Bluetooth Discovery...");
            await EnableIncomingConnectionsAsync();
            await RegisterAgentAsync();
            await RegisterSerialProfileAsync();

            var isDiscoverable = await _adapter.GetDiscoverableAsync();
            var isPowered = await _adapter.GetPoweredAsync();
            var isPairable = await _adapter.GetPairableAsync();
            var adapterName = await GetAdapterNameAsync();
            var adapterAddress = await GetAdapterAddressAsync();
            var isDiscovering = await IsDiscoveringAsync();
            var adapterAlias = await GetAliasAsync();
            Console.WriteLine($"Discoverable: {isDiscoverable}");
            Console.WriteLine($"Powered: {isPowered}");
            Console.WriteLine($"Pairable: {isPairable}");
            Console.WriteLine($"Adapter Name: {adapterName}");
            Console.WriteLine($"Adapter Address: {adapterAddress}");
            Console.WriteLine($"Discovering: {isDiscovering}");
            Console.WriteLine($"Adapter Alias: {adapterAlias}");
            Console.WriteLine("Bluetooth Discovery started.");


            await WatchInterfacesAddedAsync(info =>
            {
                Console.WriteLine($"New interface added: {info.Item1}");
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("Stopping Bluetooth Discovery...");
            return StopDiscoveryAsync();
        }
    }
}
