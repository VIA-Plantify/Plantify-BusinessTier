using Entities.Plant;

namespace Entities.Utils;

public static class TemperatureUtility
{
    public static double ConvertScale(TemperatureScale from, TemperatureScale to, double value)
    {
        if (from == to) return value;

        return (from, to) switch
        {
            (TemperatureScale.C, TemperatureScale.F) => value * 1.8 + 32,
            (TemperatureScale.F, TemperatureScale.C) => (value - 32) / 1.8,
            _ => throw new ArgumentOutOfRangeException(nameof(to), "Scale not supported")
        };
    }
}