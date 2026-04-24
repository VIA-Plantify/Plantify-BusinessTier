namespace Entities.Plant;

public interface IAirHumidity
{
    double? CurrentAirHumidity { get; set; }
    IList<double?> PastAirHumidityReadings { get; set; }
}