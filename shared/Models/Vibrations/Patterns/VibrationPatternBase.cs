using System.Diagnostics;

namespace shared.Models.Vibrations.Patterns;
public abstract record VibrationPatternBase : IVibrationPattern
{
    public double Resolution { get; set; }
    public VibrationPatternBase(double resolution)
    {
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