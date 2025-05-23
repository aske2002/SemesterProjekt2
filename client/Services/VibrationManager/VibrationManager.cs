using System.Device.Pwm;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using client.Models;
using shared.Models.Vibrations;
using shared.Models.Vibrations.Patterns;
using System.Diagnostics;
using System.Collections.Concurrent;

public class VibrationManager : IHostedService, IRecipient<SetVibrationSettingsMessage>, IRecipient<ToggleVibrationsMessage>
{
    private ConcurrentDictionary<Guid, VibrationSettings> cachedVibrationSettings = new ConcurrentDictionary<Guid, VibrationSettings>();

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
            HardwareConstants.VIBRATION_CHIP_PIN_NR,
            HardwareConstants.VIBRATION_PWM_FREQUENCY,
            0
        );
    }

    private void TimerTicked(object? state)
    {
        try
        {

            var intensity = Pattern.GetCurrentIntensity(stopwatch.Elapsed.TotalMilliseconds);
            var dutyCycle = intensity.AsDutyCycle(HardwareConstants.VIBRATION_PWM_MIN_DUTY_CYCLE, HardwareConstants.VIBRATION_PWM_MAX_DUTY_CYCLE);
            _logger.LogInformation("Vibration intensity: {Intensity} (Duty Cycle: {DutyCycle})", intensity, dutyCycle);

            lock (pwmChannel)
            {
                pwmChannel.DutyCycle = dutyCycle;
                if (!stopwatch.IsRunning)
                    return; // vibration might have stopped
                pwmChannel.Start(); // safe even if already started
            }
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

    public async void Receive(SetVibrationSettingsMessage message)
    {
        message.Reply(Task.CompletedTask);
        VibrationSettings = await VibrationSettings.FromBytes(message.Value)
            ?? throw new ArgumentException("Invalid vibration settings");
        _logger.LogInformation("Vibration settings changed to pattern with id {Id} of type {Type} with resolution {Resolution}", VibrationSettings.Id, VibrationSettings.Pattern.GetType(), VibrationSettings.Pattern.Resolution);
    }

    public void Receive(ToggleVibrationsMessage message)
    {
        VibrationEnabled = message.Value;
        message.Reply(Task.CompletedTask);
    }
}