public static class MathExtensions
{
    public static decimal Remap(this decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    public static int Remap(this int value, int fromSource, int toSource, int fromTarget, int toTarget)
    {
        return (int)((value - fromSource) / (double)(toSource - fromSource) * (toTarget - fromTarget) + fromTarget);
    }

    public static double Remap(this double value, double fromSource, double toSource, double fromTarget, double toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}