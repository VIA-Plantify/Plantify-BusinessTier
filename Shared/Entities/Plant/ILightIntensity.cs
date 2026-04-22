namespace Entities.Plant;

public interface ILightIntensity
{
    double? CurrentLightIntensity { get; set; }
    TimeSpan? CurrentLightPeriod { get; set; }
    IList<double?> PastLightIntensityReadings { get; set; }
    IList<TimeSpan?> PastLightPeriodReadings { get; set; }
}