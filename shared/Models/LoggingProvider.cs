using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace shared.Models;
public class CustomLoggingProvider : ILoggerProvider
{
    private static ILoggerFactory? _loggerFactory;
    public static ILoggerFactory LoggerFactory
    {
        get
        {
            if (_loggerFactory == null)
            {
                _loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
                {
                    builder
                        .SetMinimumLevel(LogLevel.Information)
                        .AddConsole();
                    builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DebugLoggerProvider>());
                }) ?? throw new InvalidOperationException("LoggerFactory cannot be null");
                return _loggerFactory;

            }
            return _loggerFactory;
        }
    }
    public static ILogger<T> CreateLogger<T>()
    {
        return LoggerFactory.CreateLogger<T>();
    }

    public void AddProvider(ILoggerProvider provider)
    {
        LoggerFactory.AddProvider(provider);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return LoggerFactory.CreateLogger(categoryName);
    }

    public void Dispose()
    {
        LoggerFactory.Dispose();
    }
}