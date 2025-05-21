using Microsoft.Extensions.Logging;

namespace shared.Models.Vibrations.Patterns;


public record VibrationPatternDynamic : VibrationPatternBase
{
    private const int SegmentDurationSize = 4; // 4 bytes for duration
    private const int SegmentIntensitySize = 8; // 8 bytes for intensity
    private const int SegmentSize = SegmentDurationSize + SegmentIntensitySize; // Total size of a segment in bytes

    public record VibrationPatternSegment(int DurationMS, double Intensity);
    public override VibrationMode Mode => VibrationMode.Dynamic;
    public double Duration => Segments.Sum(x => x.DurationMS);
    private Dictionary<double, VibrationPatternSegment> durationMap { get; set; } = new Dictionary<double, VibrationPatternSegment>();
    public List<VibrationPatternSegment> Segments
    {
        get => durationMap.Values.ToList();
        set
        {
            var map = new Dictionary<double, VibrationPatternSegment>();
            double currentDuration = 0;
            foreach (var segment in value)
            {
                map[currentDuration] = segment;
                currentDuration += segment.DurationMS;
            }
            durationMap = map;
        }
    }
    public override double GetIntensityValue(double time)
    {
        var loopTime = time % Duration;
        var segment = durationMap.FirstOrDefault(x => x.Key >= loopTime).Value;
        return segment == null ? 0 : segment.Intensity;
    }

    public VibrationPatternDynamic(List<VibrationPatternSegment> segments, double resolution) : base(resolution)
    {
        Segments = segments.ToList();
    }

    /// <summary>
    /// Parses a byte array into a VibrationPatternDynamic object.
    /// </summary>
    /// <param name="data">The byte array to parse, must be a multiple of 4 bytes, and at least 4 bytes long.</param>
    /// <param name="resolution"></param>
    /// <returns> A Task that represents the asynchronous operation. The task result contains the VibrationPatternDynamic object.</returns>
    /// <exception cref="ArgumentException">Thrown when the byte array is not valid.</exception>
    public static Task<VibrationPatternDynamic> ParseAsync(PatternReader reader, double resolution)
    {
        if (reader.RemainingBytes < SegmentSize || reader.RemainingBytes % SegmentSize != 0)
        {
            throw new ArgumentException("Data must be a multiple of 4 bytes and at least 4 bytes long.");
        }

        var segments = new List<VibrationPatternSegment>();
        while (!reader.IsEmpty)
        {
            var duration = reader.ReadInt32();
            var intensity = reader.ReadDouble();
            segments.Add(new VibrationPatternSegment(duration, intensity));

        }
        return Task.FromResult(new VibrationPatternDynamic(segments, resolution));
    }
    public override byte[] GetDataBytes()
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        foreach (var segment in Segments)
        {
            writer.Write(segment.DurationMS);
            writer.Write(segment.Intensity);
        }
        writer.Flush();
        var result = stream.ToArray();
        stream.Dispose();
        writer.Dispose();
        return result;
    }
}