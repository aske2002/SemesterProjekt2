using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using shared.Models.Vibrations;
namespace shared.Models.Vibrations.Patterns;

public record VibrationPatternExpression : VibrationPatternBase
{
    public override VibrationMode Mode => VibrationMode.Expression;
    private Func<double, double> _compiledFunction;
    private Expression<Func<double, double>> _function;
    public override double GetIntensityValue(double time)
    {
        return _compiledFunction(time);
    }

    public VibrationPatternExpression(Expression<Func<double, double>> function, double resolution) : base(resolution)
    {
        _function = function;
        _compiledFunction = function.Compile();
        this.Resolution = estimateRefreshInterval();
    }

    /// <summary>
    /// Parses a byte array into a VibrationPatternExpression object,
    /// /// </summary>
    /// <param name="data">The byte array to parse, there is no length restriction, but it should be a valid expression.</param>
    /// <param name="resolution">The resolution of the vibration pattern, default is double.MaxValue.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the VibrationPatternExpression object.</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is invalid.</exception>
    public static async Task<VibrationPatternExpression> ParseAsync(BinaryAdapter reader, double resolution)
    {
        var expression = reader.ReadAllAsString();
        return await ParseAsync(expression, resolution);
    }

    //expression kunne e.g v�re "Math.Sin(t * 2 * Math.PI / 1000) * 0.5 + 0.5"
    public static async Task<VibrationPatternExpression> ParseAsync(string stringExpression, double resolution)
    {
        // Compile it using Roslyn
        var options = ScriptOptions.Default
            .AddReferences([typeof(Math).Assembly, typeof(Expression).Assembly])
            .AddImports("System", "System.Math", "System.Linq", "System.Linq.Expressions", "System.Collections.Generic");

        var expression = await CSharpScript.EvaluateAsync<Expression<Func<double, double>>>(stringExpression, options);
        return new VibrationPatternExpression(expression, resolution);
    }
    public override byte[] GetDataBytes()
    {
        var stringExpression = _function.ToString();
        var data = System.Text.Encoding.UTF8.GetBytes(stringExpression);
        return data;
    }
    private double estimateRefreshInterval()
    {
        double maxRefreshInterval = 1000;

        double maxAllowedDelta = 0.05; // 5% of the range
        double testDurationMs = 1000;
        double stepMs = 1;

        double maxSlope = 0;
        double lastValue = _compiledFunction(0);
        for (double t = stepMs; t <= testDurationMs; t += stepMs)
        {
            double currentValue = _compiledFunction(t);
            if (currentValue < 0 || currentValue > 1)
            {
                throw new ArgumentException(string.Format("Intensity out of bounds: {0} at simulated time {1}", currentValue, t));
            }

            double slope = Math.Abs((currentValue - lastValue) / stepMs);
            if (slope > maxSlope)
                maxSlope = slope;

            lastValue = currentValue;
        }

        if (maxSlope == 0)
            return maxRefreshInterval; // Very slow change, can update rarely

        // Compute a refresh interval that keeps delta below the threshold
        double interval = maxAllowedDelta / maxSlope;
        return Math.Max(interval, 1); // Clamp to at least 1ms
    }
}