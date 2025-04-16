using System.Diagnostics;

namespace shared.Models.Vibrations.Patterns;
public abstract record VibrationPatternBase : IVibrationPattern
{
    private double _resolution;
    public double Resolution
    {
        get => _resolution;
        set
        {
            _resolution = value;
            Recalculate();
        }
    }
    public VibrationPatternBase(double resolution)
    {
        _stopwatch.Start();
        this._resolution = resolution;
    }
    public VibrationIntensity CurrentIntensity => new VibrationIntensity(GetIntensityValue());
    internal double time => _stopwatch.Elapsed.TotalMilliseconds;
    private Stopwatch _stopwatch = new Stopwatch();
    public abstract VibrationMode Mode { get; }
    public abstract double GetIntensityValue();
    public abstract byte[] ToBytes();
    internal abstract void Recalculate();
}