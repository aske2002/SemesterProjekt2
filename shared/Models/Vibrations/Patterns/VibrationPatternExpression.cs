using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using shared.Models.Vibrations;
namespace shared.Models.Vibrations.Patterns;

public record VibrationPatternExpression : VibrationPatternBase
{
    public override VibrationMode Mode => VibrationMode.Expression;
    private Func<double, double> _function;
    private string expression;

    public override double GetIntensityValue()
    {

        return _function(time);
    }

    public VibrationPatternExpression(Func<double, double> function, string expression, double resolution) : base(resolution)
    {
        _function = function;
        this.expression = expression;
    }

    /// <summary>
    /// Parses a byte array into a VibrationPatternExpression object,
    /// /// </summary>
    /// <param name="data">The byte array to parse, there is no length restriction, but it should be a valid expression.</param>
    /// <param name="resolution">The resolution of the vibration pattern, default is double.MaxValue.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the VibrationPatternExpression object.</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is invalid.</exception>
    public static async Task<VibrationPatternExpression> ParseAsync(byte[] data, double resolution)
    {
        var expression = System.Text.Encoding.UTF8.GetString(data);
        return await ParseAsync(expression, resolution);
    }

    //expression kunne e.g være "Math.Sin(t * 2 * Math.PI / 1000) * 0.5 + 0.5"
    public static async Task<VibrationPatternExpression> ParseAsync(string expression, double resolution)
    {
        // Compile it using Roslyn
        var options = ScriptOptions.Default
            .AddReferences(typeof(Math).Assembly)
            .AddImports("System", "System.Math");
        string code = $"(double t) => {expression}";
        var compiled = await CSharpScript.EvaluateAsync<Func<double, double>>(code, options);

        try
        {
            // Test the compiled function with a sample value
            var testValue = compiled(0);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid expression: {expression}", ex);
        }

        return new VibrationPatternExpression(compiled, expression, resolution);
    }

    internal override void Recalculate() { }

    public override byte[] ToBytes()
    {
        var data = System.Text.Encoding.UTF8.GetBytes(expression);
        return data;
    }
}