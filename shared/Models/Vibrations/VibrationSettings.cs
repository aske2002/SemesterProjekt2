using System.Collections.Concurrent;
using System.Linq.Expressions;
using shared.Models.Vibrations.Patterns;

namespace shared.Models.Vibrations;

public record VibrationSettings
{
    private static ConcurrentDictionary<Guid, VibrationSettings> _cachedVibrationSettings = new ConcurrentDictionary<Guid, VibrationSettings>();
    public required IVibrationPattern Pattern;
    public Guid Id;

    public static VibrationSettings Default => new VibrationSettings
    {
        Id = Guid.NewGuid(),
        Pattern = new VibrationPatternConstant(0)
    };

    public static VibrationSettings CreateMixedPatternSettings(params (VibrationSettings settings, int durationMS)[] patterns)
    {
        var pattern = new VibrationPatternMixed(patterns.Select(p => new VibrationPatternMixed.VibrationPatternSegment(p.settings.Pattern, p.durationMS)).ToList(), 5);
        return new VibrationSettings { Pattern = pattern, Id = Guid.NewGuid() };
    }

    /// <summary>
    /// Creates a VibrationSettings object with a sine pattern. 
    /// </summary>
    /// <param name="frequency">
    /// The frequency of the sine wave in Hz.
    /// </param>
    /// <returns>VibrationSettings object</returns>
    public static VibrationSettings CreateSinePatternSettings(double frequency)
    {
        var t = Expression.Parameter(typeof(double), "t");
        var freqConst = Expression.Constant(2 * Math.PI * frequency);
        var divisor = Expression.Constant(1000.0);
        var half = Expression.Constant(0.5);

        var timeDiv = Expression.Divide(t, divisor);
        var angle = Expression.Multiply(freqConst, timeDiv);
        var sin = Expression.Call(typeof(Math).GetMethod("Sin", [typeof(double)]), angle);
        var scaled = Expression.Add(Expression.Multiply(sin, half), half);

        var expr = Expression.Lambda<Func<double, double>>(scaled, t);

        return new VibrationSettings
        {
            Id = Guid.NewGuid(),
            Pattern = new VibrationPatternExpression(expr, 1)
        };
    }

    /// <summary>
    /// Creates a VibrationSettings object with a constant pattern.
    /// </summary>
    /// <param name="intensity">
    /// Vibration intensity from 0.0 to 1.0 (0% to 100%).
    /// </param>
    /// <returns>VibrationSettings object</returns>
    public static VibrationSettings CreateConstantPatternSettings(double intensity)
    {
        var pattern = new VibrationPatternConstant(intensity);
        return new VibrationSettings
        {
            Id = Guid.NewGuid(),
            Pattern = pattern
        };
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
    public static VibrationSettings CreateDynamicPatternSettings(params (int durationMS, double intensity)[] points)
    {
        var segments = points
            .Select(point => new VibrationPatternDynamic.VibrationPatternSegment(point.durationMS, point.intensity))
            .ToList();
        var pattern = new VibrationPatternDynamic(segments, 20);
        return new VibrationSettings
        {
            Id = Guid.NewGuid(),
            Pattern = pattern
        };
    }
    public byte[] ToBytes()
    {
        return Id.ToByteArray()
            .Concat(Pattern.ToBytes())
            .ToArray();
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
    public static async Task<VibrationSettings?> FromBytes(byte[] bytes)
    {
        if (bytes.Length == 0)
        {
            return null;
        }

        var reader = PatternReader.Create(bytes);
        var id = reader.ReadGuid();

        if (_cachedVibrationSettings.TryGetValue(id, out var cachedSettings))
        {
            reader.Dispose();
            return cachedSettings;
        }

        var pattern = await VibrationHelpers.ParseAsVibrationData(reader);
        reader.Dispose();

        var settings = new VibrationSettings
        {
            Id = id,
            Pattern = pattern
        };
        _cachedVibrationSettings.TryAdd(id, settings);
        return settings;
    }
}