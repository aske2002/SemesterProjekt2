using System.Runtime.CompilerServices;

namespace tremorur.Services;

public interface IMessenger
{
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

    void SendMessage<TMessage>(
        TMessage message,
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "",
        [CallerLineNumber] int callerLineNumber = 0)
        where TMessage : class;
}
