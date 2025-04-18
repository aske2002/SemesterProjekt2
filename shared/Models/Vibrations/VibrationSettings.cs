using shared.Models.Vibrations.Patterns;

namespace shared.Models.Vibrations;
public record VibrationSettings
{
    public required IVibrationPattern Pattern;
    public Guid Id;

    public static VibrationSettings Default => new VibrationSettings
    {
        Id = Guid.NewGuid(),
        Pattern = new VibrationPatternConstant(0)
    };

    /// <summary>
    /// Creates a VibrationSettings object with a sine pattern.
    /// </summary>
    /// <param name="frequency">
    /// The frequency of the sine wave in Hz.
    /// </param>
    /// <returns>VibrationSettings object</returns>
    public static async Task<VibrationSettings> CreateSinePatternSettings(float frequency)
    {
        var expression = $"Math.Sin(t * 2 * Math.PI * {frequency} / 1000) * 0.5 + 0.5";
        var pattern = await VibrationPatternExpression.ParseAsync(expression, 1);
        return new VibrationSettings
        {
            Id = Guid.NewGuid(),
            Pattern = pattern
        };
    }

    /// <summary>
    /// Creates a VibrationSettings object with a constant pattern.
    /// </summary>
    /// <param name="intensity">
    /// Vibration intensity from 0.0 to 1.0 (0% to 100%).
    /// </param>
    /// <returns>VibrationSettings object</returns>
    public static Task<VibrationSettings> CreateConstantPatternSettings(double intensity)
    {
        var pattern = new VibrationPatternConstant(intensity, double.MaxValue);
        return Task.FromResult(new VibrationSettings
        {
            Id = Guid.NewGuid(),
            Pattern = pattern
        });
    }

    /// <summary>
    /// Creates a VibrationSettings object with a dynamic pattern.
    /// </summary>
    /// <param name="points">An array of points (durationMS, intensity)</param>
    /// <example>
    /// VibrationSettings.CreateDynamicPatternSettings((10, 0.5), (100, 0.25), (10000, 1.0));
    /// Creates a dynamic pattern with three segments:
    ///     50% intensity for 10ms,
    ///     25% intensity for 100ms,
    ///     100% intensity for 10000ms.
    /// </example>
    /// <returns>VibrationSettings object</returns>
    public static Task<VibrationSettings> CreateDynamicPatternSettings(params (double durationMS, double intensity)[] points)
    {
        var segments = points
            .Select(point => new VibrationPatternDynamic.VibrationPatternSegment((int)point.durationMS, point.intensity))
            .ToList();
        var pattern = new VibrationPatternDynamic(segments, 1);
        return Task.FromResult(new VibrationSettings
        {
            Id = Guid.NewGuid(),
            Pattern = pattern
        });
    }
    public byte[] ToBytes()
    {
        var idBytes = Id.ToByteArray();
        var resolutionBytes = BitConverter.GetBytes(Pattern.Resolution);
        var dataBytes = Pattern.ToBytes();


        byte[] bytes = new byte[1] { (byte)Pattern.Mode }
            .Concat(idBytes)
            .Concat(resolutionBytes)
            .Concat(dataBytes)
            .ToArray();
        return bytes;
    }

    /// <summary>
    /// Converts a byte array to a VibrationSettings object.
    /// The byte array should be in the format:
    /// <list type="bullet">
    /// <item>
    /// <description>byte 0: VibrationMode</description>
    /// </item>
    /// <item>
    /// <description>byte 1-16: Id</description>
    /// </item>
    /// <item>
    /// <description>byte 17-24: Resolution</description>
    /// </item>
    /// <item>
    /// <description>byte 25-: Data (At least 2 bytes)</description>
    /// </item>
    /// </list>
    /// <b>Note: The byte array must be at least 27 bytes long.</b>
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the VibrationSettings object.</returns>
    /// <exception cref="ArgumentException">Thrown when the byte array is not valid.</exception>
    /// <exception cref="ArgumentException">Thrown when the pattern is not valid.</exception>
    public static async Task<VibrationSettings> FromBytes(byte[] bytes)
    {
        if (bytes.Length < 27)
        {
            throw new ArgumentException("Byte array must be at least 27 bytes long.");
        }

        var mode = (VibrationMode)bytes[0];
        var id = new Guid(bytes.Skip(1).Take(16).ToArray());
        var resolution = BitConverter.ToDouble(bytes, 17);
        var data = bytes.Skip(25).ToArray();
        var pattern = await VibrationDataFactory.ParseAsVibrationData(data, mode, resolution);

        return new VibrationSettings
        {
            Id = id,
            Pattern = pattern
        };
    }
}