namespace tremorur.Services;

public interface IMiddlewareRegistration
{
    Type MessageType { get; }

    object Execute(object message);
}

public class MiddlewareRegistration<TMessage> : IMiddlewareRegistration
    where TMessage : class
{
    public Type MessageType => typeof(TMessage);

    private readonly Func<TMessage, TMessage> _middleware;

    public MiddlewareRegistration(Func<TMessage, TMessage> middleware)
    {
        _middleware = middleware;
    }

    public object Execute(object message)
    {
        if (message is TMessage typedMessage)
        {
            return _middleware(typedMessage);
        }

        return message;
    }
}