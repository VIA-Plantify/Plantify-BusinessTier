namespace Entities.Plant;

public class AirHumidity : IAirHumidity
{
    private double _optimalAirHumidity;
    private double? _currentAirHumidity;

    public double? CurrentAirHumidity
    {
        get => _currentAirHumidity;
        set
        {
            if (value.HasValue && (value < 0 || value > 100))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Humidity must be between 0 and 100.");
            }
            _currentAirHumidity = value;
        }
    }

    public double OptimalAirHumidity
    {
        get => _optimalAirHumidity;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Humidity must be between 0 and 100.");
            }
            _optimalAirHumidity = value;
        }
    } 
    public int AirHumidityPercentage 
    {
        get 
        {
            return (int)Math.Round(CurrentAirHumidity ?? 0.0);
        }
    }
    public IList<double?> PastAirHumidityReadings { get; set; } = new List<double?>(); 
}
