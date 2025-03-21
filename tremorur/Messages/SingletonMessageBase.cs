namespace tremorur.Messages;

public abstract record SingletonMessageBase<TSelf>
    where TSelf : new()
{
    public static readonly TSelf Instance = new();
}
