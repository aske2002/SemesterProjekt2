using System.Device.Pwm;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using client.Models;
using shared.Models.Vibrations;
using shared.Models.Vibrations.Patterns;
using System.Diagnostics;

public class VibrationManager : IHostedService, IRecipient<SetVibrationSettingsMessage>, IRecipient<ToggleVibrationsMessage>
{
    private readonly IMessenger _messenger;
    private readonly ILogger<VibrationManager> _logger;
    private bool _isVibrationEnabled = false;
    private VibrationSettings _vibrationSettings = VibrationSettings.Default;
    private Stopwatch stopwatch = new Stopwatch();
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
        var intensity = Pattern.GetCurrentIntensity(stopwatch.Elapsed.TotalMilliseconds);
        var dutyCycle = intensity.AsDutyCycle(HardwareConstants.VIBRATION_PWM_MIN_DUTY_CYCLE, HardwareConstants.VIBRATION_PWM_MAX_DUTY_CYCLE);
        _logger.LogInformation("Vibration intensity: {Intensity} (Duty Cycle: {DutyCycle})", intensity, dutyCycle);
        pwmChannel.DutyCycle = Math.Max(0.0, Math.Min(1.0, dutyCycle));

        try
        {
            pwmChannel.Start();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting PWM channel");
            return;
        }
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

        stopwatch.Restart();
        _logger.LogInformation("Starting vibration timer with resolution {Resolution}", resolution);
        _timer = new Timer(TimerTicked, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(resolution));
    }

    private void SetVibration()
    {
        if (VibrationEnabled)
        {
            _logger.LogInformation("Vibration enabled");
            StartInterval(Pattern.Resolution);
            pwmChannel.Start();
        }
        else
        {
            _logger.LogInformation("Vibration disabled");
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
        _logger.LogInformation("Vibration settings changed to pattern with id {Id} of type {Type} with resolution {Resolution}", message.Value.Id, message.Value.Pattern.GetType(), message.Value.Pattern.Resolution);
        message.Reply(Task.CompletedTask);
    }

    public void Receive(ToggleVibrationsMessage message)
    {
        VibrationEnabled = message.Value;
        message.Reply(Task.CompletedTask);
    }
}