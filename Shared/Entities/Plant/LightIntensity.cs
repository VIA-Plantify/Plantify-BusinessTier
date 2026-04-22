namespace Entities.Plant;

public class LightIntensity : ILightIntensity
{
    private double? currentIntensity;
    private TimeSpan? currentPeriod;
    private IList<double?> pastIntensityReadings = new List<double?>();
    private IList<TimeSpan?> pastPeriodReadings = new List<TimeSpan?>();

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

    public IList<double?> PastLightIntensityReadings
    {
        get => pastIntensityReadings;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(PastLightIntensityReadings), "Past light readings is null");
            }
            pastIntensityReadings = value;
        }
    }

    public IList<TimeSpan?> PastLightPeriodReadings

    {
        get => pastPeriodReadings;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(PastLightPeriodReadings), "Past period readings is null");
            }
            pastPeriodReadings = value;
        }
    }

    public double OptimalLightIntensity { get; set; }
    public TimeSpan OptimalLightPeriod { get; set; }
}