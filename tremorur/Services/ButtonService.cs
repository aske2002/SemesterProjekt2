
using tremorur.Messages;

public class ButtonClickedEventArgs : EventArgs
{
    public string ButtonName { get; }

    public ButtonClickedEventArgs(string buttonName)
    {
        ButtonName = buttonName;
    }
}

public interface IButtonService
{
    event EventHandler? UpButtonClicked;
    event EventHandler? DownButtonClicked;
    event EventHandler? OkButtonClicked;
    event EventHandler? CancelButtonClicked;
}

public class ButtonService : IButtonService //hvis bottonservice skal bruges et sted, skriver man bottonservice : IbottonService 
{
    private readonly tremorur.Services.IMessenger _messenger; //constructor der tager en Imessage som parameter 
    public ButtonService(tremorur.Services.IMessenger messenger)
    {
        _messenger = messenger;
        Initialize();
    }

    public void Initialize() //initialize metode 
    {
        _messenger.LytEfterBegivenhed<ButtonClickedEvent>((ButtonClickedEvent e) =>
        {
            switch (e.Button)
            {
                case WatchButton.Up:
                    UpButtonClicked?.Invoke(this, EventArgs.Empty);
                    break;
                case WatchButton.Down:
                    DownButtonClicked?.Invoke(this, EventArgs.Empty);
                    break;
                case WatchButton.Ok:
                    OkButtonClicked?.Invoke(this, EventArgs.Empty);
                    break;
                case WatchButton.Cancel:
                    CancelButtonClicked?.Invoke(this, EventArgs.Empty);
                    break;
            }
        });
    }

    public event EventHandler? UpButtonClicked;
    public event EventHandler? DownButtonClicked;
    public event EventHandler? OkButtonClicked;
    public event EventHandler? CancelButtonClicked;
}