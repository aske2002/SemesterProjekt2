
//

using shared.Models.Vibrations.Patterns;

namespace shared.Models.Vibrations;
public static class VibrationDataFactory
{
    public static async Task<IVibrationPattern> ParseAsVibrationData(byte[] data, VibrationMode mode, double resolution)
    {
        return mode switch
        {
            VibrationMode.Dynamic => await VibrationPatternDynamic.ParseAsync(data, resolution),
            VibrationMode.Expression => await VibrationPatternExpression.ParseAsync(data, resolution),
            VibrationMode.Constant => await VibrationPatternConstant.ParseAsync(data, resolution),
            _ => throw new NotImplementedException($"Vibration mode {mode} is not implemented"),
        };
    }
}

