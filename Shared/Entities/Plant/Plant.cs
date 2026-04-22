namespace Entities.Plant;

public class Plant
{
    public ITemperature Temperature { get; set; }
    public TemperatureScale currentScale { get; set; } = TemperatureScale.C;

    public async Task<ITemperature> ConvertTemperature(TemperatureScale scale)
    {
        if (currentScale == scale)
        {
            return await Task.FromResult(Temperature);
        }

        Temperature.OptimalTemperature = ConvertScale(scale, Temperature.OptimalTemperature);
        if (Temperature.CurrentTemperature is not null)
        {
            Temperature.CurrentTemperature = ConvertScale(scale, Temperature.CurrentTemperature);
        }

        for (int i = 0; i < Temperature?.PastTemperatureReadings.Count; i++)
        {
            Temperature.PastTemperatureReadings[i] = ConvertScale(scale, Temperature.PastTemperatureReadings[i]);
        }

        currentScale = scale;
        return await Task.FromResult(Temperature) ?? throw new InvalidOperationException("Something went wrong with the Temperature");
    }

    private double ConvertScale(TemperatureScale scale, double? value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "Value is not given");
        }
        switch (scale)
        {
            case TemperatureScale.F:
                return (double)((value - 32) / 1.8);
            case TemperatureScale.C:
                return (double)(value * 1.8 + 32);
            default:
                throw new ArgumentOutOfRangeException(nameof(scale), scale, "Scale not supported");
        }
    }
}