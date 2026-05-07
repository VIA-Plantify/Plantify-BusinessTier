using Entities.Utils;

namespace Entities.Plant;

public class SensorData
{
    public long Id { get; set; }
    
    private double _temperature;
    private double _airHumidity;
    private double _soilHumidity;
    private double _lightIntensity;
    
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string PlantMAC { get; set; } = string.Empty;

    public double Temperature
    {
        get => _temperature;
        set => _temperature = value;
    }

    public double AirHumidity
    {
        get => _airHumidity;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Humidity must be between 0 and 100.");
            _airHumidity = value;
        }
    }

    public double SoilHumidity
    {
        get => _soilHumidity;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Humidity must be between 0 and 100.");
            _soilHumidity = value;
        }
    }

    public double LightIntensity
    {
        get => _lightIntensity;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Light intensity cannot be negative.");
            _lightIntensity = value;
        }
    }
    
    public int? TemperatureDeviationPercent(double optimalTemperature, TemperatureScale dataScale, TemperatureScale optimalScale)
    {
        if (optimalTemperature == 0) return null;

        double current = _temperature;
        if (dataScale != optimalScale)
            current = TemperatureUtility.ConvertScale(dataScale, optimalScale, current);

        return PercentUtility.CalculateDeviationPercent(current, optimalTemperature);
    }

    public int? AirHumidityDeviationPercent(double optimalAirHumidity) =>
        optimalAirHumidity == 0
            ? null
            : PercentUtility.CalculateDeviationPercent(_airHumidity, optimalAirHumidity);

    public int? SoilHumidityDeviationPercent(double optimalSoilHumidity) =>
        optimalSoilHumidity == 0
            ? null
            : PercentUtility.CalculateDeviationPercent(_soilHumidity, optimalSoilHumidity);

    public int? LightIntensityDeviationPercent(double optimalLightIntensity) =>
        optimalLightIntensity == 0
            ? null
            : PercentUtility.CalculateDeviationPercent(_lightIntensity, optimalLightIntensity);
}