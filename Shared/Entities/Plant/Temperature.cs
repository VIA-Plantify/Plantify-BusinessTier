using Entities.Utils;

namespace Entities.Plant;

public class Temperature
{
    private double? _value;
    public double? Value
    {
        get => _value;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(_value), "Temperature is null");
            }
            
            if (value.HasValue && (value < 0 || value > 200))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Temperature must be between 0 and 200.");
            }
            _value = value;
        }
    }

    public IList<double?> PastReadings { get; set; } = new List<double?>();
    
    public TemperatureScale Scale { get; set; } = TemperatureScale.C;
    
    public Temperature ConvertTemperature(TemperatureScale scale)
    {
        if (Scale == scale)
            return this;

        if (Value is not null)
            Value = TemperatureUtility.ConvertScale(Scale, scale, Value.Value);

        for (int i = 0; i < PastReadings.Count; i++)
        {
            if (PastReadings[i] is not null)
                PastReadings[i] = TemperatureUtility.ConvertScale(Scale, scale, PastReadings[i]!.Value);
        }

        Scale = scale;
        return this;
    }
    
}