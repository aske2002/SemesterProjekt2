using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using tremorur.Messages;
using ICommunityToolkitMessenger = CommunityToolkit.Mvvm.Messaging.IMessenger;

namespace tremorur.Services;

public class Messenger : IMessenger
{
    private readonly ILogger _logger;
    private readonly ICommunityToolkitMessenger _messenger = StrongReferenceMessenger.Default;
    private readonly List<IMiddlewareRegistration> _middleware = new();
    private IServiceProvider? serviceProvider;

    public Messenger(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        On<AppBuilt>(m => serviceProvider = m.Services);
    }

    public IDisposable On<TMessage>(
        Action<TMessage> handler,
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "",
        [CallerLineNumber] int callerLineNumber = 0)
        where TMessage : class
    {
        _logger.Log(LogLevel.Information, "On<{MessageType}>({Handler}, {CallerFilePath}, {CallerMemberName}, {CallerLineNumber})",
                            typeof(TMessage), handler, callerFilePath, callerMemberName, callerLineNumber);

        var recipient = new object();

        _messenger.Register<TMessage>(recipient, (_, m) =>
        {
            var message = (TMessage)ExecuteMiddleware(m);
            handler(message);
        });

        return new Unsubscriber<TMessage>(_messenger, recipient);
    }

    public IDisposable On<TMessage>(Action handler)
        where TMessage : class
        => On<TMessage>(_ => handler());

    public IDisposable OnOnce<TMessage>(Action handler) where TMessage : class
    {
        IDisposable? unsub = null;
        unsub = On<TMessage>(() =>
        {
            handler();
            unsub?.Dispose();
        });

        return unsub;
    }

    public void SendMessage<TMessage>(
        TMessage message,
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "",
        [CallerLineNumber] int callerLineNumber = 0)
        where TMessage : class
    {
        _logger.Log(LogLevel.Information, "Send<{MessageType}>({Message}, {CallerFilePath}, {CallerMemberName}, {CallerLineNumber})",
                            typeof(TMessage), message, callerFilePath, callerMemberName, callerLineNumber);

        _messenger.Send(message);
    }


}

sealed file class Unsubscriber<T>(ICommunityToolkitMessenger messenger, object recipient) : IDisposable
    where T : class
{
    public void Dispose()
    {
        messenger.Unregister<T>(recipient);
    }
}