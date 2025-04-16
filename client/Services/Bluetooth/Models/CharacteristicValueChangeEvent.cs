using client.Models;

namespace client.Services.Bluetooth.Models;
public record CharChangeData
{
    public string DeviceId { get; init; } = string.Empty;
    public string CharacteristicId { get; init; } = string.Empty;
    public byte[] Data { get; init; } = Array.Empty<byte>();
}


public class CharChangeEvent : AsyncRequestResponseMesage<CharChangeData, MessageResponse>
{
    public CharChangeEvent(CharChangeData request) : base(request)
    {
    }
}