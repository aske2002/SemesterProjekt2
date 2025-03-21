using System.Runtime.CompilerServices;

namespace tremorur.Services;

public interface IMessenger
{
    IDisposable RegisterMiddlewareFor<TMessage>(
        Func<TMessage, TMessage> middleware,
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "",
        [CallerLineNumber] int callerLineNumber = 0)
        where TMessage : class;

    IDisposable On<TMessage>(
        Action<TMessage> handler,
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "",
        [CallerLineNumber] int callerLineNumber = 0)
        where TMessage : class;

    IDisposable On<TMessage>(Action handler)
        where TMessage : class
        => On<TMessage>(_ => handler());

    IDisposable OnOnce<TMessage>(Action handler)
        where TMessage : class
        => On<TMessage>(_ => handler());

    void Send<TMessage>(
        TMessage message,
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "",
        [CallerLineNumber] int callerLineNumber = 0)
        where TMessage : class;
}
