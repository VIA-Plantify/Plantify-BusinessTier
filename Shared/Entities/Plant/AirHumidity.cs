namespace Entities.Plant;

public class AirHumidity
{
    private double? _value;

    public double? Value
    {
        get => _value;
        set
        {
            if (value.HasValue && (value < 0 || value > 100))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Humidity must be between 0 and 100.");
            }
            _value = value;
        }
    }
    public IList<double?> PastReadings { get; set; } = new List<double?>(); 
}
