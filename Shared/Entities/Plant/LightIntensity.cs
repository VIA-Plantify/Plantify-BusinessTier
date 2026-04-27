namespace Entities.Plant;

public class LightIntensity
{
    private double? _value;
    private TimeSpan? _period;
    public double? Value
    {
        get => _value;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(_value), "Light intensity is null");
            }
            if (value < 0)
            {
                throw new ArgumentException("Light intensity cannot be negative");
            }
            _value = value;
        }
    }

    public TimeSpan? Period
    {
        get => _period;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(_value), "Light period is null");
            }
            _period= value;
        }
    }

    public IList<double?> PastReadings { get; set; } = new List<double?>();

    public IList<TimeSpan?> PastPeriods { get; set; } = new List<TimeSpan?>();
    
}