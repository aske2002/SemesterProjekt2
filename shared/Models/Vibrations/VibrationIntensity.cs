
namespace shared.Models.Vibrations;

public record VibrationIntensity
{
    public double PercentageValue;
    public double AsDutyCycle(double minDutyCycle = 0.0, double maxDutyCycle = 1.0) => PercentageValue * (maxDutyCycle - minDutyCycle) + minDutyCycle;
    public VibrationIntensity(double percentageValue)
    {
        PercentageValue = Math.Clamp(percentageValue, 0, 1);
    }
}