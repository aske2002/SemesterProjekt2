using System.Diagnostics;

namespace shared.Models.Vibrations.Patterns;
public abstract record VibrationPatternBase : IVibrationPattern
{
    public const int MAX_RESOLUTION = 10000;
    public const int MIN_RESOLUTION = 1;
    public double Resolution { get; set; }
    public VibrationPatternBase(double resolution)
    {
        if (resolution > MAX_RESOLUTION || resolution < MIN_RESOLUTION)
        {
            throw new ArgumentOutOfRangeException(nameof(resolution), "Resolution must be less than 60000 milliseconds.");
        }
        Resolution = resolution;
    }
    public VibrationIntensity GetCurrentIntensity(double time)
    {
        var intensity = GetIntensityValue(time);
        return new VibrationIntensity(intensity);
    }
    public abstract VibrationMode Mode { get; }
    public abstract double GetIntensityValue(double time);
    public abstract byte[] GetDataBytes();
}