using System;
using System.Threading.Tasks;
using client.Services.Bluetooth.Utilities;
using Tmds.DBus;

namespace client.Services.Bluetooth.Core
{
    public abstract class PropertiesBase<TV> where TV : notnull
    {
        protected readonly TV Properties;

        protected PropertiesBase(ObjectPath objectPath, TV properties)
        {
            ObjectPath = objectPath;
            Properties = properties;
        }

        public ObjectPath ObjectPath { get; }

        public Task<object> GetAsync(string prop)
        {
            return Task.FromResult(Properties.ReadProperty(prop));
        }
        public Task<T> GetAsync<T>(string prop)
        {
            return Task.FromResult(Properties.ReadProperty<T>(prop));
        }

        public Task<TV> GetAllAsync()
        {
            return Task.FromResult(Properties);
        }


        public Task SetAsync(string prop, object val)
        {
            OnPropertiesChanged.Invoke(PropertyChanges.ForProperty(prop, val));
            return Properties.SetProperty(prop, val);
        }

        public Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler)
        {
            return SignalWatcher.AddAsync(this, nameof(OnPropertiesChanged), handler);
        }
        public event Action<PropertyChanges> OnPropertiesChanged = delegate { };
    }
}