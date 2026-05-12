using Entities.Plant;

namespace DTOs.Plant;

public record PlantDto
{
    public required string MAC { get; init; }
    public required string Name { get; init; }
    public required string Username { get; init; }

    // Current sensor data
    public SensorDataDto SensorData { get; init; } = new();
    public List<SensorDataDto> PreviousSensorData { get; init; } = [];

    // Watering data
    public WateringDto Watering { get; init; } = new();
    public List<WateringDto> PreviousWaterings { get; init; } = [];

    // Optimal values
    public required double OptimalTemperature { get; init; }
    public required double OptimalAirHumidity { get; init; }
    public required double OptimalSoilHumidity { get; init; }
    public required double OptimalLightIntensity { get; init; }

    public TemperatureScale Scale { get; init; }
}