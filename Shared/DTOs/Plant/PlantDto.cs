using Entities.Plant;

namespace DTOs.Plant;

public record PlantDto
{
    public required string MAC { get; init; }
    public required string Name { get; init; }
    public required string Username { get; init; }

    // Current sensor data
    public SensorData SensorData { get; init; } = new SensorData();
    public List<SensorData> PreviousSensorData { get; init; } = [];

    // Watering data
    public Watering Watering { get; init; } = new Watering();
    public List<Watering> PreviousWaterings { get; init; } = [];

    // Optimal values
    public required double OptimalTemperature { get; init; }
    public required double OptimalAirHumidity { get; init; }
    public required double OptimalSoilHumidity { get; init; }
    public required double OptimalLightIntensity { get; init; }
    public required long OptimalLightPeriod { get; init; } // seconds

    // Temperature scale
    public TemperatureScale Scale { get; init; }

    // Deviation percentages
    public int? TemperatureDeviationPercent { get; init; }
    public int? AirHumidityDeviationPercent { get; init; }
    public int? SoilHumidityDeviationPercent { get; init; }
    public int? LightIntensityDeviationPercent { get; init; }
}