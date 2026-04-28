namespace Entities.Plant;

public class SoilHumidity
{
    private double? _value;

    public double? Value
    {
        get => _value;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Soil humidity cannot be negative");
            }

            _value = value;
        }
    }

    public IList<double?> PastReadings { get; set; } = new List<double?>();
}