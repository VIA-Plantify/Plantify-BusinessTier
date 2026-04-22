using Entities.Utils;

namespace Entities.Plant;

public class Plant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ITemperature Temperature { get; set; } = new Temperature();
    public IAirHumidity AirHumidity { get; set; } = new AirHumidity();
    public IWaterLevel WaterLevel { get; set; } = new WaterLevel();
    public IWaterIntake WaterIntake { get; set; } = new WaterIntake();
    public IHumidity SoilHumidity { get; set; } = new SoilHumidity();
    public ILightIntensity LightIntensity { get; set; } = new LightIntensity();

    private double _optimalTemperature;
    private double _optimalAirHumidity;
    private double _optimalSoilHumidity;
    private double _optimalLightIntensity;
    private TimeSpan _optimalLightPeriod;
    private TemperatureScale _temperatureScale = TemperatureScale.C;

    public TemperatureScale TemperatureScale
    {
        get => _temperatureScale;
        set
        {
            if (_temperatureScale == value)
                return;

            _optimalTemperature = TemperatureUtility.ConvertScale(_temperatureScale, value, _optimalTemperature);
            Temperature.ConvertTemperature(value);
            _temperatureScale = value;
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
            if (Temperature.CurrentTemperature is null || OptimalTemperature == 0)
                return null;

            double current = Temperature.CurrentTemperature.Value;

            if (Temperature.CurrentScale != TemperatureScale)
            {
                current = TemperatureUtility.ConvertScale(
                    Temperature.CurrentScale,
                    TemperatureScale,
                    current
                );
            }

            return PercentUtility.CalculateDeviationPercent(current, OptimalTemperature);
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
        AirHumidity.CurrentAirHumidity is null || OptimalAirHumidity == 0
            ? null
            : PercentUtility.CalculateDeviationPercent(
                AirHumidity.CurrentAirHumidity,
                OptimalAirHumidity
            );

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
        SoilHumidity.CurrentSoilHumidity is null || OptimalSoilHumidity == 0
            ? null
            : PercentUtility.CalculateDeviationPercent(
                SoilHumidity.CurrentSoilHumidity,
                OptimalSoilHumidity
            );

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
        LightIntensity.CurrentLightIntensity is null || OptimalLightIntensity == 0
            ? null
            : PercentUtility.CalculateDeviationPercent(
                LightIntensity.CurrentLightIntensity,
                OptimalLightIntensity
            );

    public TimeSpan OptimalLightPeriod
    {
        get => _optimalLightPeriod;
        set
        {
            if (value < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(value), "Light period cannot be negative.");

            _optimalLightPeriod = value;
        }
    }
}