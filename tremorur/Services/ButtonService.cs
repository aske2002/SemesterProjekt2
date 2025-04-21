
using tremorur.Messages;

public interface IButtonService
{
    event EventHandler<ButtonClickedEventArgs> OnButtonClicked;
    event EventHandler<ButtonMultipleClickedEventArgs> OnButtomMultipleClicked;
}

public record ButtonMultipleClickedEventArgs(WatchButton Button, int ClickCount);
public record ButtonClickedEventArgs(WatchButton Button);

public class ButtonService : IButtonService
{
    private Dictionary<WatchButton, int> _clickCounts = new();
    private Dictionary<WatchButton, Timer> _clickTimers = new();
    private const int ClickDelay = 500;

    private readonly tremorur.Services.IMessenger _messenger;
    public ButtonService(tremorur.Services.IMessenger messenger)
    {
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _messenger.On<ButtonClickedMessage>(Button_Clicked);
    }

    private void FireClickedEvent(object? state)
    {
        if (state is not WatchButton button)
            return;

        if (_clickCounts.TryGetValue(button, out var _clickCount) && _clickCount == 1)
        {
            OnButtonClicked?.Invoke(this, new ButtonClickedEventArgs(button));
        }
        else if (_clickCount > 1)
        {
            OnButtomMultipleClicked?.Invoke(this, new ButtonMultipleClickedEventArgs(button, _clickCount));
        }

        _clickTimers.GetValueOrDefault(button)?.Dispose();
        _clickTimers.Remove(button);
        _clickCounts.Remove(button);
    }

    private void Button_Clicked(ButtonClickedMessage e)
    {
        _clickCounts[e.Button] = _clickCounts.GetValueOrDefault(e.Button) + 1;
        if (_clickTimers.TryGetValue(e.Button, out var existingTimer))
        {
            existingTimer.Change(ClickDelay, Timeout.Infinite);
        }
        else
        {
            _clickTimers[e.Button] = new Timer(FireClickedEvent, e.Button, ClickDelay, Timeout.Infinite);
        }
    }


    public event EventHandler<ButtonClickedEventArgs> OnButtonClicked = delegate { };
    public event EventHandler<ButtonMultipleClickedEventArgs> OnButtomMultipleClicked = delegate { };
}