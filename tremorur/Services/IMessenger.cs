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

    IDisposable LytEfterBegivenhed<TMessage>(
        Action<TMessage> handler,
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "",
        [CallerLineNumber] int callerLineNumber = 0)
        where TMessage : class;

    IDisposable LytEfterBegivenhed<TMessage>(Action handler)
        where TMessage : class
        => LytEfterBegivenhed<TMessage>(_ => handler());

    IDisposable OnOnce<TMessage>(Action handler)
        where TMessage : class
        => LytEfterBegivenhed<TMessage>(_ => handler());

    void SendBegivenhed<TMessage>(
        TMessage message,
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "",
        [CallerLineNumber] int callerLineNumber = 0)
        where TMessage : class;
}
