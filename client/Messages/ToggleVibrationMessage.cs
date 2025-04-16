using CommunityToolkit.Mvvm.Messaging.Messages;

namespace client.Models;
public class ToggleVibrationsMessage : AsyncRequestResponseMesage<bool>
{
    public ToggleVibrationsMessage(bool request) : base(request)
    {
    }
}