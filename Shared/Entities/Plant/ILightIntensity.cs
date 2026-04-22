namespace Entities.Plant;

public interface ILightIntensity
{
    double? CurrentLightIntensity { get; set; }
    TimeSpan? CurrentLightPeriod { get; set; }
    IList<double?> PastLightIntensityReadings { get; set; }
    IList<double?> PastLightPeriodReadings { get; set; }
    double OptimalLightIntensity { get; set; }
    
    //I would also suggest adding Optimal Light Period
}