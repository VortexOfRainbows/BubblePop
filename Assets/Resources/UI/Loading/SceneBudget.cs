using System.Diagnostics;

public static class SceneBudget
{
    private static Stopwatch FrameWatch = Stopwatch.StartNew();
    public const float MaxTimeMs = 12f;

    public static bool ShouldYield()
    {
        if (FrameWatch.ElapsedMilliseconds >= MaxTimeMs)
        {
            FrameWatch.Restart();
            return true;
        }
        return false;
    }
}
