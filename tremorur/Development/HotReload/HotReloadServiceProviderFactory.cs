using System.Reflection;

namespace tremorur.Development.HotReload;

public class HotReloadServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    public IServiceProvider CreateServiceProvider(IServiceCollection services)
    {
        var originalProvider = services.BuildServiceProvider();

        return new HotReloadServiceProvider(originalProvider);
    }
}

public class HotReloadServiceScopeFactory : IServiceScopeFactory
{
    private readonly IServiceScopeFactory _inner;

    public HotReloadServiceScopeFactory(IServiceScopeFactory inner)
    {
        _inner = inner;
    }

    public IServiceScope CreateScope()
    {
        var scope = _inner.CreateScope();
        return new HotReloadServiceScope(scope);
    }
}

public class HotReloadServiceScope : IServiceScope
{
    private readonly IServiceScope _inner;

    public HotReloadServiceScope(IServiceScope inner)
    {
        _inner = inner;
    }

    public IServiceProvider ServiceProvider => new HotReloadServiceProvider(_inner.ServiceProvider);

    public void Dispose()
    {
        _inner.Dispose();
    }
}

public class HotReloadServiceProvider : IServiceProvider
{
    private readonly IServiceProvider _inner;

    public HotReloadServiceProvider(IServiceProvider inner)
    {
        _inner = inner;
    }

    public IServiceScopeFactory CreateScope()
    {
        var scopeFactory = _inner.GetService(typeof(IServiceScopeFactory)) as IServiceScopeFactory;
        if (scopeFactory != null)
        {
            return new HotReloadServiceScopeFactory(scopeFactory);
        }

        return null;
    }
    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(IServiceScopeFactory))
        {
            return CreateScope();
        }

        var instance = _inner.GetService(serviceType);

        if (instance != null)
        {
            TryRegisterDeep(instance, new HashSet<object>());
        }

        return instance;
    }

    private void TryRegisterDeep(object obj, HashSet<object> visited)
    {
        if (obj == null || visited.Contains(obj))
            return;

        visited.Add(obj);

        var type = obj.GetType();

        if (HasOnHotReloadMethod(type))
        {
            HotReloadInstanceTracker.Register(obj);
        }


        // If it's a visual element, dive into children
        if (obj is VisualElement visualElement)
        {
            foreach (var child in GetAllVisualDescendants(visualElement))
            {
                TryRegisterDeep(child, visited);
            }
        }
    }

    private bool HasOnHotReloadMethod(Type type)
    {
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return methods.Any(m => m.GetCustomAttributes(typeof(OnHotReloadAttribute), true).Any());
    }

    private IEnumerable<Element> GetAllVisualDescendants(Element parent)
    {
        if (parent is IElementController controller)
        {
            foreach (var child in controller.LogicalChildren)
            {
                yield return child;

                foreach (var grandChild in GetAllVisualDescendants(child))
                    yield return grandChild;
            }
        }
    }
}
