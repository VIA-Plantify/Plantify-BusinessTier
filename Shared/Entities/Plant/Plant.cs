using Entities.Utils;

namespace Entities.Plant;

public class Plant
{
    
    public string MAC { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public required string Username { get; set; }

    
    public SensorData SensorData { get; set; } = new SensorData();
    public List<SensorData> PreviousSensorData { get; set; } = [];

    
    public Watering Watering { get; set; } = new Watering();
    public List<Watering> PreviousWaterings { get; set; } = [];

    private double _optimalTemperature;
    private double _optimalAirHumidity;
    private double _optimalSoilHumidity;
    private double _optimalLightIntensity;
    private TemperatureScale _scale = TemperatureScale.C;

    public TemperatureScale Scale
    {
        get => _scale;
        set
        {
            if (_scale == value)
                return;

            _optimalTemperature = TemperatureUtility.ConvertScale(_scale, value, _optimalTemperature);
            _scale = value;
        }
    }

    public double OptimalTemperature
    {
        get => _optimalTemperature;
        set => _optimalTemperature = value;
    }
    

    public double OptimalAirHumidity
    {
        get => _optimalAirHumidity;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Humidity must be between 0 and 100.");

            _optimalAirHumidity = value;
        }
    }

    public double OptimalSoilHumidity
    {
        get => _optimalSoilHumidity;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Humidity must be between 0 and 100.");

            _optimalSoilHumidity = value;
        }
    }

    public double OptimalLightIntensity
    {
        get => _optimalLightIntensity;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Light intensity cannot be negative.");

            _optimalLightIntensity = value;
        }
    }
}