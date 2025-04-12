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
            await SetDiscoverableAsync(true);
            await SetPoweredAsync(true);
            await SetPairableAsync(true);
            await SetAliasAsync("Tremor-ur");
            await StartDiscoveryAsync();

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
