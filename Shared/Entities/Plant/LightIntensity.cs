namespace Entities.Plant;

public class LightIntensity : ILightIntensity
{
    private double? currentIntensity;
    private TimeSpan? currentPeriod;

    public double? CurrentLightIntensity
    {
        get => currentIntensity;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(CurrentLightIntensity), "Light intensity is null");
            }
            if (value < 0)
            {
                throw new ArgumentException("Light intensity cannot be negative");
            }
            currentIntensity = value;
        }
    }

    public TimeSpan? CurrentLightPeriod
    {
        get => currentPeriod;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(CurrentLightPeriod), "Light period is null");
            }
            currentPeriod = value;
        }
    }

    public IList<double?> PastLightIntensityReadings { get; set; } = new List<double?>();

    public IList<TimeSpan?> PastLightPeriodReadings { get; set; } = new List<TimeSpan?>();
    
}