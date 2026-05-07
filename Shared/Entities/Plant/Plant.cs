using Entities.Utils;

namespace Entities.Plant;

public class Plant
{
    // MAC address for the arduino network
    public string MAC { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public required string Username { get; set; }

    // Sensor data
    public SensorData SensorData { get; set; } = new SensorData();
    public List<SensorData> PreviousSensorData { get; set; } = [];

    // Watering data
    public Watering Watering { get; set; } = new Watering();
    public List<Watering> PreviousWaterings { get; set; } = [];

    private double _optimalTemperature;
    private double _optimalAirHumidity;
    private double _optimalSoilHumidity;
    private double _optimalLightIntensity;
    private long _optimalLightPeriod; // bigint → seconds
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

    public int? TemperatureDeviationPercent
    {
        get
        {
            if (SensorData.Temperature == 0 || OptimalTemperature == 0)
                return null;

            return PercentUtility.CalculateDeviationPercent(SensorData.Temperature, OptimalTemperature);
        }
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

    public int? AirHumidityDeviationPercent =>
        SensorData.AirHumidity == 0 || OptimalAirHumidity == 0
            ? null
            : PercentUtility.CalculateDeviationPercent(SensorData.AirHumidity, OptimalAirHumidity);

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

    public int? SoilHumidityDeviationPercent =>
        SensorData.SoilHumidity == 0 || OptimalSoilHumidity == 0
            ? null
            : PercentUtility.CalculateDeviationPercent(SensorData.SoilHumidity, OptimalSoilHumidity);

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

    public int? LightIntensityDeviationPercent =>
        SensorData.LightIntensity == 0 || OptimalLightIntensity == 0
            ? null
            : PercentUtility.CalculateDeviationPercent(SensorData.LightIntensity, OptimalLightIntensity);

    public long OptimalLightPeriod
    {
        get => _optimalLightPeriod;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Light period cannot be negative.");

            _optimalLightPeriod = value;
        }
    }
}