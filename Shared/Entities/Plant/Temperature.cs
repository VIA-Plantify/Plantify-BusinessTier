namespace Entities.Plant;

public class Temperature : ITemperature
{
    
    public double? CurrentTemperature { get; set; }
    public IList<double?> PastTemperatureReadings { get; set; }
    public double OptimalTemperature { get; set; }
    
    public Temperature()
    {
        PastTemperatureReadings = new List<double?>();
    }
}