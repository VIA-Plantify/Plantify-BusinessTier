namespace Entities.Utils;

public static class PercentUtility
{
    public static int CalculateDeviationPercent(double? current, double optimal)
    {
        if (current is null || optimal == 0)
            return 0;

        return (int)Math.Round(((current.Value - optimal) / optimal) * 100);
    }
}