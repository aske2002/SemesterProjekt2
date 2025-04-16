using System.Device.Pwm;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using client.Models;
using shared.Models.Vibrations;
using shared.Models.Vibrations.Patterns;

public class VibrationManager : IHostedService, IRecipient<SetVibrationSettingsMessage>, IRecipient<ToggleVibrationsMessage>
{
    private readonly IMessenger _messenger;
    private readonly ILogger<VibrationManager> _logger;
    private bool _isVibrationEnabled = false;
    private VibrationSettings _vibrationSettings = VibrationSettings.Default;
    public bool VibrationEnabled
    {
        get => _isVibrationEnabled;
        set
        {
            _isVibrationEnabled = value;
            SetVibration();
        }
    }

    public IVibrationPattern Pattern => _vibrationSettings.Pattern;

    public VibrationSettings VibrationSettings
    {
        get => _vibrationSettings;
        set
        {
            _vibrationSettings = value;
            SetVibration();
        }
    }

    private Timer? _timer;
    private PwmChannel pwmChannel;
    public VibrationManager(IMessenger messenger, ILogger<VibrationManager> logger)
    {
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _messenger.RegisterAll(this);
        pwmChannel = PwmChannel.Create(
            HardwareConstants.GPIO_CHIP,
            HardwareConstants.VIBRATION_PWM_GPIO,
            HardwareConstants.VIBRATION_PWM_FREQUENCY,
            0
        );
    }

    private void TimerTicked(object? state)
    {
        var intensity = Pattern.CurrentIntensity;
        pwmChannel.DutyCycle = intensity.AsDutyCycle(HardwareConstants.VIBRATION_PWM_MIN_DUTY_CYCLE, HardwareConstants.VIBRATION_PWM_MAX_DUTY_CYCLE);
    }

    private void ClearInterval()
    {
        _timer?.Dispose();
        _timer = null;
    }

    private void StartInterval(double resolution)
    {
        if (_timer != null)
        {
            ClearInterval();
        }

        _timer = new Timer(TimerTicked, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(resolution));
    }

    private void SetVibration()
    {
        if (VibrationEnabled)
        {
            StartInterval(Pattern.Resolution);
        }
        else
        {
            pwmChannel.Stop();
            ClearInterval();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _messenger.UnregisterAll(this);
        _timer?.Dispose();
        _timer = null;
        pwmChannel?.Stop();
        pwmChannel?.Dispose();
        return Task.CompletedTask;
    }

    public void Receive(SetVibrationSettingsMessage message)
    {
        VibrationSettings = message.Value;
        message.Reply(Task.CompletedTask);
    }

    public void Receive(ToggleVibrationsMessage message)
    {
        VibrationEnabled = message.Value;
        message.Reply(Task.CompletedTask);
    }
}