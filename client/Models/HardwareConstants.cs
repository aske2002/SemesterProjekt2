using shared.Models;

namespace client.Models;

public static class HardwareConstants
{
    public const int VIBRATION_CHIP_PIN_NR = 0; // Physical pin 32
    public const int GPIO_CHIP = 0;
    public const double VIBRATION_PWM_MIN_DUTY_CYCLE = 0.3f;
    public const double VIBRATION_PWM_MAX_DUTY_CYCLE = 1.0f;
    public const int VIBRATION_PWM_FREQUENCY = 200;
    public const int UP_BUTTON_GPIO = 5;     // Physical pin 29
    public const int DOWN_BUTTON_GPIO = 6;   // Physical pin 31
    public const int OK_BUTTON_GPIO = 13;     // Physical pin 33
    public const int CANCEL_BUTTON_GPIO = 19; // Physical pin 35
    public static readonly Dictionary<int, WatchButton> BUTTON_GPIO_MAPPING = new()
    {
        { UP_BUTTON_GPIO, WatchButton.Up },
        { DOWN_BUTTON_GPIO, WatchButton.Down },
        { OK_BUTTON_GPIO, WatchButton.Ok },
        { CANCEL_BUTTON_GPIO, WatchButton.Cancel }
    };
}