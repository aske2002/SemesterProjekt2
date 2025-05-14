
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using shared.Messages;
using shared.Models;
using tremorur.Messages;
using tremorur.Models.Bluetooth;
using tremorur.Models.Bluetooth.Events;

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
    private readonly ILogger<ButtonService> _logger;
    private readonly IBluetoothStateManager _bluetoothStateManager;
    public ButtonService(tremorur.Services.IMessenger messenger, ILogger<ButtonService> logger, IBluetoothStateManager bluetoothStateManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _bluetoothStateManager = bluetoothStateManager ?? throw new ArgumentNullException(nameof(bluetoothStateManager));
        _messenger.On<ButtonPressedMessage>(Button_Pressed);
        _messenger.On<ButtonReleasedMessage>(Button_Released);

        _bluetoothStateManager.CharacteristicValueChanged += ButtonValueChanged;
        _bluetoothStateManager.DiscoveredCharacteristic += Characteristic_Discovered;
        _bluetoothStateManager.PeripheralConnected += Peripheral_Connected;
    }

    private async void Peripheral_Connected(object? sender, IBluetoothPeripheral e)
    {
        var service = e.Services.FirstOrDefault(s => s.UUID == BluetoothIdentifiers.ButtonServiceUUID);

        if (service == null)
            return;

        foreach (var characteristic in service.Characteristics)
        {
            if (BluetoothIdentifiers.ButtonStateCharacteristicUUIDs.ContainsKey(characteristic.UUID))
            {
                _logger.LogInformation($"Discovered button characteristic: {characteristic.UUID}");
                await characteristic.SetNotifyingAsync(true);
            }

        }
    }

    private void Characteristic_Discovered(object? sender, DiscoveredCharacteristicEventArgs e)
    {
        if (e.Service.UUID != BluetoothIdentifiers.ButtonServiceUUID)
            return;

        e.Characteristic.SetNotifyingAsync(true);
    }

    private void ButtonValueChanged(object? sender, CharacteristicValueChangedEventArgs e)
    {
        if (e.Service.UUID != BluetoothIdentifiers.ButtonServiceUUID)
            return;

        if (!BluetoothIdentifiers.ButtonStateCharacteristicUUIDs.TryGetValue(e.Characteristic.UUID, out var button))
            return;

        if (!ButtonStateChanged.TryParse(e.Value, out var buttonState))
            return;

        _logger.LogInformation($"Button {buttonState.State} state changed to {buttonState.State}");
        switch (buttonState.State)
        {
            case ButtonState.Pressed:
                _messenger.SendMessage(new ButtonPressedMessage(button));
                break;
            case ButtonState.Depressed:
                _messenger.SendMessage(new ButtonReleasedMessage(buttonState.Button));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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