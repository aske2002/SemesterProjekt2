
using tremorur.Messages;

public interface IButtonService
{
    event EventHandler<ButtonClickedEventArgs> OnButtonClicked;
    event EventHandler<ButtonMultipleClickedEventArgs> OnButtomMultipleClicked;
    event EventHandler<ButtonHeldEventArgs> OnButtonHeld;
    void Hold_Handled(WatchButton e);
}

public record ButtonMultipleClickedEventArgs(WatchButton Button, int ClickCount);
public record ButtonClickedEventArgs(WatchButton Button);
public record ButtonHeldEventArgs(WatchButton Button, int HeldMS);
public class ButtonService : IButtonService
{
    private Dictionary<WatchButton, int> _clickCounts = new();
    private Dictionary<WatchButton, Timer> _holdTimers = new();
    private Dictionary<WatchButton, Timer> _clickTimers = new();
    private const int ClickDelay = 250;
    private const int HoldDelay = 1000;
    private const int HoldDelayCheckInterval = 100;

    private readonly tremorur.Services.IMessenger _messenger;
    public ButtonService(tremorur.Services.IMessenger messenger)
    {
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _messenger.On<ButtonPressedMessage>(Button_Pressed);
        _messenger.On<ButtonReleasedMessage>(Button_Released);
    }

    public void Hold_Handled(WatchButton btn)
    {
        if (_holdTimers.TryGetValue(btn, out var timer))
        {
            timer.Dispose();
            _holdTimers.Remove(btn);
        }
    }
    private void FireMultiClickedEvent(object? state)
    {
        if (state is not WatchButton btn)
            return;

        if (_clickCounts.TryGetValue(btn, out var clickCount) && clickCount > 1)
        {
            OnButtomMultipleClicked?.Invoke(this, new ButtonMultipleClickedEventArgs(btn, clickCount));
        }
        _clickCounts.Remove(btn);
    }

    private void ResetClick(WatchButton btn)
    {
        _clickCounts.Remove(btn);
        _clickTimers.GetValueOrDefault(btn)?.Dispose();
        _clickTimers.Remove(btn);
    }

    private void FireHoldEvent(object? state)
    {
        if (state is not ButtonHeldEventArgs evt)
            return;

        ResetClick(evt.Button);
        OnButtonHeld?.Invoke(this, new ButtonHeldEventArgs(evt.Button, evt.HeldMS + HoldDelayCheckInterval));
        _holdTimers[evt.Button] = new Timer(FireHoldEvent, new ButtonHeldEventArgs(evt.Button, evt.HeldMS + HoldDelayCheckInterval), HoldDelayCheckInterval, Timeout.Infinite);
    }

    private void Button_Pressed(ButtonPressedMessage e)
    {
        _holdTimers[e.Button] = new Timer(FireHoldEvent, new ButtonHeldEventArgs(e.Button, HoldDelay), HoldDelay, Timeout.Infinite);
        _clickCounts[e.Button] = _clickCounts.GetValueOrDefault(e.Button) + 1;
    }

    private void Button_Released(ButtonReleasedMessage e)
    {
        if (_holdTimers.TryGetValue(e.Button, out var timer))
        {
            timer.Dispose();
            _holdTimers.Remove(e.Button);
        }
        if (_clickCounts.TryGetValue(e.Button, out var clickCount))
        {
            OnButtonClicked?.Invoke(this, new ButtonClickedEventArgs(e.Button));
            _clickTimers.GetValueOrDefault(e.Button)?.Dispose();
            _clickTimers[e.Button] = new Timer(FireMultiClickedEvent, e.Button, ClickDelay, Timeout.Infinite);
        }
    }

    public event EventHandler<ButtonClickedEventArgs> OnButtonClicked = delegate { };
    public event EventHandler<ButtonMultipleClickedEventArgs> OnButtomMultipleClicked = delegate { };
    public event EventHandler<ButtonHeldEventArgs> OnButtonHeld = delegate { };
}