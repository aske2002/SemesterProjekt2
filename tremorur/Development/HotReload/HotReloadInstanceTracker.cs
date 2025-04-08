namespace tremorur.Development.HotReload;
public static class HotReloadInstanceTracker
{
    private static readonly Dictionary<Type, List<WeakReference>> _instances = new();

    public static void Register(object instance)
    {
        var type = instance.GetType();

        if (!_instances.TryGetValue(type, out var list))
        {
            list = new List<WeakReference>();
            _instances[type] = list;
        }

        list.Add(new WeakReference(instance));
    }

    public static IEnumerable<object> GetInstancesOfType(Type type)
    {
        if (_instances.TryGetValue(type, out var list))
        {
            foreach (var weakRef in list.ToList())
            {
                if (weakRef.Target is object target)
                    yield return target;
            }
        }
    }
}
