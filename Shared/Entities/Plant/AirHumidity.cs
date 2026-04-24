namespace Entities.Plant;

public class AirHumidity : IAirHumidity
{
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
    public IList<double?> PastAirHumidityReadings { get; set; } = new List<double?>(); 
}
