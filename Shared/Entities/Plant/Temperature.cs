using Entities.Utils;

namespace Entities.Plant;

public class Temperature : ITemperature
{
    
    public double? CurrentTemperature { get; set; }
    public IList<double?> PastTemperatureReadings { get; set; } = new List<double?>();
    
    public TemperatureScale CurrentScale { get; set; } = TemperatureScale.C;
    
    public ITemperature ConvertTemperature(TemperatureScale scale)
    {
        if (CurrentScale == scale)
            return this;

        if (CurrentTemperature is not null)
            CurrentTemperature = TemperatureUtility.ConvertScale(CurrentScale, scale, CurrentTemperature.Value);

        for (int i = 0; i < PastTemperatureReadings.Count; i++)
        {
            if (PastTemperatureReadings[i] is not null)
                PastTemperatureReadings[i] = TemperatureUtility.ConvertScale(CurrentScale, scale, PastTemperatureReadings[i]!.Value);
        }

        CurrentScale = scale;
        return this;
    }
    
}