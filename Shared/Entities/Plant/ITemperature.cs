namespace Entities.Plant;

public interface ITemperature
{
    double CurrentTemperature { get; set; }
    IList<double> PastTemperatureReadings { get; set; }
    double OptimalTemperature { get; set; }
}