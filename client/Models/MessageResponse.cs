namespace client.Models;

public record MessageResponse : MessageResponse<object> { }

public record MessageResponse<T>
{
    public bool Error { get; init; } = false;
    public T Value { get; init; } = default!;
    public string Message { get; init; } = string.Empty;
    public static MessageResponse Success() => new MessageResponse { Error = true };
    public static MessageResponse Failure(string message) => new MessageResponse { Error = false, Message = message };
}