namespace shared.Models.Vibrations.Patterns;


public record VibrationPatternConstant : VibrationPatternBase
{
    public override VibrationMode Mode => VibrationMode.Constant;
    private double intensity;

    public override double GetIntensityValue()
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
    public static Task<VibrationPatternConstant> ParseAsync(byte[] data, double resolution = double.MaxValue)
    {
        if (data.Length != 2)
        {
            throw new ArgumentException("Data must be 2 bytes long.");
        }

        var intensity = BitConverter.ToInt16(data, 0) / (double)short.MaxValue;
        return Task.FromResult(new VibrationPatternConstant(intensity, resolution));
    }

    internal override void Recalculate() { }

    public override byte[] ToBytes()
    {
        var data = BitConverter.GetBytes((short)(intensity * short.MaxValue));
        return data;
    }
}