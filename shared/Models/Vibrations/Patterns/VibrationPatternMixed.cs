using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using shared.Models.Vibrations;
namespace shared.Models.Vibrations.Patterns;

public record VibrationPatternMixed : VibrationPatternBase
{
    private enum MixedPatternFlags
    {
        Pattern = 0,
        DurationMap = 1,
    }
    public record VibrationPatternSegment(IVibrationPattern Pattern, int Duration);
    public override VibrationMode Mode => VibrationMode.Mixed;
    public int Duration => Segments.Sum(x => x.Duration);
    public List<VibrationPatternSegment> Segments { get; set; } = new List<VibrationPatternSegment>();
    public Dictionary<double, VibrationPatternSegment> DurationMap
    {
        get
        {
            var map = new Dictionary<double, VibrationPatternSegment>();
            double currentDuration = 0;
            foreach (var segment in Segments)
            {
                currentDuration += segment.Duration;
                map[currentDuration] = segment;
            }
            return map;
        }
    }

    public override double GetIntensityValue(double time)
    {
        var loopTime = time % Duration;
        var segment = DurationMap.FirstOrDefault(x => x.Key >= loopTime).Value;
        if (segment == null)
        {
            return 0;
        }
        return segment.Pattern.GetIntensityValue(time);
    }

    public VibrationPatternMixed(List<VibrationPatternSegment> segments, double resolution) : base(resolution)
    {
        Segments = segments;
    }

    /// <summary>
    /// Parses a byte array into a VibrationPatternExpression object,
    /// /// </summary>
    /// <param name="data">The byte array to parse, there is no length restriction, but it should be a valid expression.</param>
    /// <param name="resolution">The resolution of the vibration pattern, default is double.MaxValue.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the VibrationPatternExpression object.</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is invalid.</exception>
    public static async Task<VibrationPatternMixed> ParseAsync(PatternReader reader, double resolution)
    {
        List<IVibrationPattern> segments = new List<IVibrationPattern>();
        List<int> durations = new List<int>();
        while (true)
        {
            MixedPatternFlags flag = VibrationHelpers.FromFlagByte<MixedPatternFlags>(reader.ReadByte());
            if (flag == MixedPatternFlags.Pattern)
            {
                var pattern = await VibrationHelpers.ParseAsVibrationData(reader);
                segments.Add(pattern);
            }
            else if (flag == MixedPatternFlags.DurationMap)
            {
                while (!reader.IsEmpty)
                {
                    durations.Add(reader.ReadInt32());
                }
                break;
            }
            else
            {
                throw new ArgumentException($"Invalid flag {flag} in data.");
            }
        }

        if (segments.Count != durations.Count)
        {
            throw new ArgumentException("The number of segments and durations must be equal.");
        }
        var patternSegments = segments.Zip(durations, (pattern, duration) => new VibrationPatternSegment(pattern, duration)).ToList();
        return new VibrationPatternMixed(patternSegments, resolution);
    }
    public override byte[] GetDataBytes()
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        foreach (var segment in Segments)
        {
            writer.Write(MixedPatternFlags.Pattern.ToFlagByte());
            writer.Write(segment.Pattern.ToBytes());
        }

        writer.Write(MixedPatternFlags.DurationMap.ToFlagByte());
        foreach (var segment in Segments)
        {
            writer.Write(segment.Duration);
        }
        writer.Flush();
        var result = stream.ToArray();
        stream.Dispose();
        writer.Dispose();
        return result;
    }
}