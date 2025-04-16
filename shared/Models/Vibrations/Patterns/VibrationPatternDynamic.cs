namespace shared.Models.Vibrations.Patterns;


public record VibrationPatternDynamic : VibrationPatternBase
{
    public record VibrationPatternSegment(int DurationMS, double Intensity);
    public override VibrationMode Mode => VibrationMode.Dynamic;
    private List<VibrationPatternSegment> _segments = new List<VibrationPatternSegment>();
    public List<VibrationPatternSegment> Segments
    {
        get => _segments;
        set
        {
            _segments = value;
            Recalculate();
        }
    }
    private double[] intensities = Array.Empty<double>();

    public override double GetIntensityValue()
    {
        var indexRepeatable = (int)(time / Resolution) % intensities.Length;
        var intensity = intensities[indexRepeatable];
        return intensity;

    }

    public VibrationPatternDynamic(IEnumerable<VibrationPatternSegment> segments, double resolution) : base(resolution)
    {
        Segments = segments.ToList();
        Recalculate();
    }

    /// <summary>
    /// Parses a byte array into a VibrationPatternDynamic object.
    /// </summary>
    /// <param name="data">The byte array to parse, must be a multiple of 4 bytes, and at least 4 bytes long.</param>
    /// <param name="resolution"></param>
    /// <returns> A Task that represents the asynchronous operation. The task result contains the VibrationPatternDynamic object.</returns>
    /// <exception cref="ArgumentException">Thrown when the byte array is not valid.</exception>
    public static Task<VibrationPatternDynamic> ParseAsync(byte[] data, double resolution)
    {
        if (data.Length < 4 || data.Length % 4 != 0)
        {
            throw new ArgumentException("Data must be a multiple of 4 bytes and at least 4 bytes long.");
        }

        var segments = data.Chunk(4).Select(segment =>
         {
             var duration = BitConverter.ToInt16(segment, 0);
             var intensityShort = BitConverter.ToInt16(segment, 2);
             double intensity = intensityShort / short.MaxValue;
             return new VibrationPatternSegment(duration, intensity);
         }).ToList();
        return Task.FromResult(new VibrationPatternDynamic(segments, resolution));
    }

    internal override void Recalculate()
    {
        intensities = Segments.SelectMany(segment =>
        {
            var subSegmentsCount = (int)(segment.DurationMS / Resolution);
            return Enumerable.Repeat(segment.Intensity, subSegmentsCount);
        }).ToArray();

    }

    public override byte[] ToBytes()
    {
        var data = new byte[Segments.Count * 4];
        for (int i = 0; i < Segments.Count; i++)
        {
            var segment = Segments[i];
            Array.Copy(BitConverter.GetBytes((short)segment.DurationMS), 0, data, i * 4, 2);
            Array.Copy(BitConverter.GetBytes((short)(segment.Intensity * short.MaxValue)), 0, data, i * 4 + 2, 2);
        }
        return data;
    }
}