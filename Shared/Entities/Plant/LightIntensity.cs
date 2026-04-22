namespace Entities.Plant;

public class LightIntensity : ILightIntensity
{
    public double? CurrentLightIntensity { get; set; }
    public TimeSpan? CurrentLightPeriod { get; set; }
    public IList<double?> PastLightIntensityReadings { get; set; }
    public IList<double?> PastLightPeriodReadings { get; set; }
    public double OptimalLightIntensity { get; set; }
    
    // let me know if i should add anything else
}