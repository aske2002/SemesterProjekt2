namespace shared.Models.Vibrations.Patterns;


public record VibrationPatternConstant : VibrationPatternBase
{
    public override VibrationMode Mode => VibrationMode.Constant;
    private double intensity;

    public override double GetIntensityValue(double _)
    {
        return intensity;
    }

    public VibrationPatternConstant(double intensity, double resolution = double.MaxValue) : base(resolution)
    {
        this.intensity = intensity;
    }

    /// <summary>
    /// Parses a byte array into a VibrationPatternConstant object.
    /// </summary>
    /// <param name="data">The byte array to parse, must be 2 bytes long.</param>
    /// <param name="resolution">The resolution of the vibration pattern, default is double.MaxValue.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the VibrationPatternConstant object.</returns>
    /// <exception cref="ArgumentException">Thrown when the byte array is not valid.</exception>
    public static Task<VibrationPatternConstant> ParseAsync(BinaryAdapter reader, double resolution = double.MaxValue)
    {
        if (reader.RemainingBytes != 8)
        {
            throw new ArgumentException("Data must be at least 2 bytes long.");
        }

        var intensity = reader.ReadDouble();
        return Task.FromResult(new VibrationPatternConstant(intensity, resolution));
    }
    public override byte[] GetDataBytes()
    {
        return BitConverter.GetBytes(intensity);
    }
}