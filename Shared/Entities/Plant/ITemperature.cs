namespace Entities.Plant;

public interface ITemperature
{
    
    double? CurrentTemperature { get; set; }
    IList<double?> PastTemperatureReadings { get; set; }
    TemperatureScale CurrentScale { get; set; }

    ITemperature ConvertTemperature(TemperatureScale scale);
}