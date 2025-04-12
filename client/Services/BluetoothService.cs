using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tmds.DBus;

namespace client.Services.Bluetooth
{
    public class BluetoothService : IAsyncDisposable
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

        public async Task InitializeAsync()
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

        public async ValueTask DisposeAsync()
        {
            _interfaceAddedWatcher?.Dispose();
            _interfaceRemovedWatcher?.Dispose();
        }
    }
}
