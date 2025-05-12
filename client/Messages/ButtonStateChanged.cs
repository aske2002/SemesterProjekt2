using client.Models;
using shared.Models;
namespace shared.Messages;
public class ButtonStateChangedMessage : AsyncRequestResponseMesage<ButtonStateChanged>
{
    public ButtonStateChangedMessage(WatchButton button, ButtonState state) : base(new ButtonStateChanged(button, state))
    {
    }
}