namespace Entities.Plant;

public interface IHumidity
{
    double? CurrentSoilHumidity {  get; set; }
    IList<double?> PastSoilHumidityReadings { get; set; }
    double OptimalSoilHumidity { get; set; }
}