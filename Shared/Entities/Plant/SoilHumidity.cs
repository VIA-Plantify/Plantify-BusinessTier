namespace Entities.Plant;

public class SoilHumidity : IHumidity
{
    public double? CurrentSoilHumidity { get; set; }
    public IList<double?> PastSoilHumidityReadings { get; set; }
    public double OptimalSoilHumidity { get; set; }
    
    // let me know if i should add anything else
}