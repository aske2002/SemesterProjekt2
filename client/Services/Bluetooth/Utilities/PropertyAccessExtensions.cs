using System.Threading.Tasks;

namespace client.Services.Bluetooth.Utilities
{
    public static class PropertyAccessExtensions
    {
        public static T ReadProperty<T>(this object o, string prop)
        {
            var propertyValue = o.GetType().GetProperty(prop)?.GetValue(o);
            return propertyValue is T value ? value : throw new InvalidCastException($"Cannot cast {propertyValue?.GetType()} to {typeof(T)}");
        }

        public static object ReadProperty(this object o, string prop)
        {
            return o.GetType().GetProperty(prop)?.GetValue(o) ?? throw new InvalidOperationException($"Property {prop} not found on {o.GetType()}");
        }

        public static Task SetProperty(this object o, string prop, object val)
        {
            o.GetType().GetProperty(prop)?.SetValue(o, val);
            return Task.CompletedTask;
        }
    }
}