namespace Entities.Plant;

public class Plant
{
    public ITemperature Temperature { get; set; }
    public TemperatureScale currentScale { get; set; }

    public async Task<ITemperature> ConvertTemperature(TemperatureScale scale)
    {
        if (currentScale == scale)
        {
            return await Task.FromResult(Temperature);
        }
        Temperature.OptimalTemperature = ConvertScale(scale, Temperature.OptimalTemperature);
        Temperature.CurrentTemperature = ConvertScale(scale, Temperature.CurrentTemperature);
        for (int i = 0; i < Temperature.PastTemperatureReadings.Count; i++)
        {
            Temperature.PastTemperatureReadings[i] = ConvertScale(scale, Temperature.PastTemperatureReadings[i]);
        }
        currentScale = scale;
        return await Task.FromResult(Temperature);
    }

    private double ConvertScale(TemperatureScale scale, double value )
    {
        switch (scale)
        {
            case TemperatureScale.F:
                return (value - 32) / 1.8;
            case TemperatureScale.C:
                return value * 1.8 + 32;
            default:
                throw new ArgumentOutOfRangeException(nameof(scale), scale, "Scale not supported");
        }
    }
}