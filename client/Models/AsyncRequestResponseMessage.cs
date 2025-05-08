using CommunityToolkit.Mvvm.Messaging.Messages;

namespace client.Models;

public record AsyncRequestProxy<Req, Res>
{
    public required Req Value { get; init; }
    public bool HasReceivedResponse { get; set; }
}
public static class AsyncRequestResponseMesageExtensions
{
    public static AsyncRequestProxy<Req, Res> AsProxy<Req, Res>(this AsyncRequestResponseMesage<Req, Res> value) where Req : class where Res : class
    {
        return new AsyncRequestProxy<Req, Res>
        {
            Value = value.Value,
            HasReceivedResponse = value.HasReceivedResponse
        };
    }
}

public abstract class AsyncRequestResponseMesage<Req, Res> : AsyncRequestMessage<Res>
    where Req : class
    where Res : class
{
    public Req Value { get; init; }
    public AsyncRequestResponseMesage(Req request) : base()
    {
        Value = request;
    }
}

public abstract class AsyncRequestResponseMesage<Req> : AsyncRequestMessage<Task>
{
    public Req Value { get; init; }
    public AsyncRequestResponseMesage(Req request) : base()
    {
        Value = request;
    }
}