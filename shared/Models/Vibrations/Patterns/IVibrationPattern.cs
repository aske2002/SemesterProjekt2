using shared.Models.Vibrations;

namespace shared.Models.Vibrations.Patterns;

public interface IVibrationPattern
{
    VibrationMode Mode { get; }
    VibrationIntensity CurrentIntensity { get; }
    double Resolution { get; set; }
    double GetIntensityValue();
    byte[] ToBytes();
}