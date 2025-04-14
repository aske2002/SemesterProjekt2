namespace client.Services.Bluetooth.Gatt.BlueZModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using client.Services.Bluetooth.Core;
    using Tmds.DBus;

    public class PropertyListener
    {
        private Connection? _connection = null;
        public delegate void PropertyChangedHandler(object sender, PropertyChanges changes);
        public event EventHandler<PropertyChanges> PropertyChanged = delegate { };
        public List<KeyValuePair<string, Action<PropertyChanges>>> PropertyChangedActions { get; } = new List<KeyValuePair<string, Action<PropertyChanges>>>();
        public List<KeyValuePair<ObjectPath, IDisposable>> PropertyChangedSubscriptions { get; } = new List<KeyValuePair<ObjectPath, IDisposable>>();

        public async Task InitializeAsync(Connection connection)
        {
            _connection = connection;
            var manager = _connection.CreateProxy<IObjectManager>("org.bluez", "/");
            var allObjects = await manager.GetManagedObjectsAsync();

            foreach (var (path, interfaces) in allObjects)
            {
                WatchDevice(path, connection);
            }

            await manager.WatchInterfacesAddedAsync((path, interfaces) =>
            {
                WatchDevice(path, connection);
            });

        }

        public void ListenForInterface<TInterface>(Action<PropertyChanges> action) where TInterface : IDBusObject
        {
            var interfaceType = typeof(TInterface);
            var DBusInterface = interfaceType.GetCustomAttributes(typeof(DBusInterfaceAttribute), true).FirstOrDefault();
            if (DBusInterface == null)
            {
                throw new InvalidOperationException($"Type {interfaceType.Name} does not have a DBusInterface attribute.");
            }
            var interfaceName = ((DBusInterfaceAttribute)DBusInterface).Name;
            PropertyChangedActions.Add(new KeyValuePair<string, Action<PropertyChanges>>(interfaceName, action));
        }
        private async void WatchDevice(ObjectPath path, Connection connection)
        {
            var props = connection.CreateProxy<IProperties>("org.bluez", path);
            var sub = await props.WatchPropertiesChangedAsync(change =>
            {
                var actions = PropertyChangedActions.Where(a => a.Key == change.Interface).ToList();
                foreach (var action in actions)
                {
                    action.Value(change);
                }
            });
            PropertyChangedSubscriptions.Add(new KeyValuePair<ObjectPath, IDisposable>(path, sub));
        }

        public void Dispose()
        {
            _connection = null;
            foreach (var subscription in PropertyChangedSubscriptions)
            {
                subscription.Value.Dispose();
            }
            PropertyChangedSubscriptions.Clear();
        }
    }
}