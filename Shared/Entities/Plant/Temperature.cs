namespace Entities.Plant;

public class Temperature : ITemperature
{
    
    public double? CurrentTemperature { get; set; }
    public IList<double?> PastTemperatureReadings { get; set; } = new List<double?>();
    public double OptimalTemperature { get; set; }
    
    public TemperatureScale currentScale { get; set; } = TemperatureScale.C;
    
    public async Task<ITemperature> ConvertTemperature(TemperatureScale scale)
    {
        if (currentScale == scale)
        {
            return await Task.FromResult(this);
        }

        this.OptimalTemperature = ConvertScale(scale, this.OptimalTemperature);
        if (this.CurrentTemperature is not null)
        {
            this.CurrentTemperature = ConvertScale(scale, this.CurrentTemperature);
        }

        for (int i = 0; i < this?.PastTemperatureReadings.Count; i++)
        {
            this.PastTemperatureReadings[i] = ConvertScale(scale, this.PastTemperatureReadings[i]);
        }

        currentScale = scale;
        return await Task.FromResult(this) ?? throw new InvalidOperationException("Something went wrong with the Temperature");
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