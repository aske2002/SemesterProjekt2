using System.Reflection;
using tremorur.Development.HotReload;

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(HotReloadService))]
namespace tremorur
{
    public static class HotReloadService
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public static event Action<Type[]?>? UpdateApplicationEvent;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        internal static void ClearCache(Type[]? types) { }
        internal static void UpdateApplication(Type[]? types)
        {
            UpdateApplicationEvent?.Invoke(types);

            if (types == null) return;

            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.Instance |
                                              BindingFlags.Public |
                                              BindingFlags.NonPublic);

                var methodsWithAttr = methods
                    .Where(m => m.GetCustomAttributes(typeof(OnHotReloadAttribute), true).Any())
                    .ToList();

                if (methodsWithAttr.Count == 0)
                    continue;

                foreach (var instance in HotReloadInstanceTracker.GetInstancesOfType(type))
                {
                    foreach (var method in methodsWithAttr)
                    {
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                method.Invoke(instance, null);
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to invoke [OnHotReload] method '{method.Name}' on {type.Name}: {ex.Message}");
                        }
                    }
                }
            }
        }

    }
}

