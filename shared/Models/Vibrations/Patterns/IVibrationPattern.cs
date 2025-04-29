using shared.Models.Vibrations;

namespace shared.Models.Vibrations.Patterns;

public interface IVibrationPattern
{
    VibrationMode Mode { get; }
    VibrationIntensity GetCurrentIntensity(double time);
    double Resolution { get; set; }
    double GetIntensityValue(double time);
    byte[] GetDataBytes();
}