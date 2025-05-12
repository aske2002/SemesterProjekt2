using System.Device.Gpio;
using System.Diagnostics;
using client.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using shared.Messages;
using shared.Models;

namespace client.Services.Button;

public class HardwareButtonManager : IHostedService
{
    private readonly Dictionary<int, Dictionary<PinEventTypes, DateTime>> _lastEventTimes = new();
    private readonly TimeSpan _debounceTime = TimeSpan.FromMilliseconds(5);
    private GpioController _gpioController = new(PinNumberingScheme.Logical);
    private ILogger<HardwareButtonManager> _logger;
    private IMessenger _messenger;
    public HardwareButtonManager(IMessenger messenger, ILogger<HardwareButtonManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messenger = messenger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var (pin, button) in HardwareConstants.BUTTON_GPIO_MAPPING)
        {
            _logger.LogInformation($"Opening GPIO pin {pin} for button {button}");
            _gpioController.OpenPin(pin, PinMode.InputPullUp);
            _gpioController.RegisterCallbackForPinValueChangedEvent(pin, PinEventTypes.Rising | PinEventTypes.Falling, OnPinEvent);
        }
        return Task.CompletedTask;
    }

    private void OnPinEvent(object? sender, PinValueChangedEventArgs e)
    {
        var now = DateTime.UtcNow;

        // Debounce logic
        if (_lastEventTimes.TryGetValue(e.PinNumber, out var lastEventDict))
        {
            if (lastEventDict.TryGetValue(e.ChangeType, out var lastEventTime))
            {
                if (now - lastEventTime < _debounceTime)
                    return; // Ignore noisy bounces
            }
            else
            {
                lastEventDict[e.ChangeType] = now;
            }
        }
        else
        {
            _lastEventTimes[e.PinNumber] = new Dictionary<PinEventTypes, DateTime>()
            {
                { e.ChangeType, now }
            };
        }


        if (!HardwareConstants.BUTTON_GPIO_MAPPING.TryGetValue(e.PinNumber, out var button))
            return;

        if (e.ChangeType == PinEventTypes.Rising)
        {
            _logger.LogInformation($"Button {button} released");
            _messenger.Send(new ButtonStateChangedMessage(button, ButtonState.Depressed));
        }
        else
        {
            _logger.LogInformation($"Button {button} pressed");
            _messenger.Send(new ButtonStateChangedMessage(button, ButtonState.Pressed));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var (pin, _) in HardwareConstants.BUTTON_GPIO_MAPPING)
        {
            _gpioController.UnregisterCallbackForPinValueChangedEvent(pin, OnPinEvent);
            _gpioController.ClosePin(pin);
        }
        return Task.CompletedTask;
    }
}