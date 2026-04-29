using Entities.Plant;

namespace DTOs.Plant;

public record PlantDto
{
    public required string MAC { get;  init; }
    public required string Name { get; init; }
    public required string Username { get; init; }
    public Temperature Temperature { get; init; }
    public AirHumidity AirHumidity { get; init; }
    public WaterLevel WaterLevel { get; init; }
    public WaterIntake WaterIntake { get; init; }
    public SoilHumidity SoilHumidity { get; init; }
    public LightIntensity LightIntensity { get; init; }
    
    public required double OptimalTemperature { get; init; }
    public required double OptimalAirHumidity { get; init; }
    public required double OptimalWaterLevel { get; init; }
    public required double OptimalWaterIntake { get; init; }
    public required double OptimalSoilHumidity { get; init; }
    public required double OptimalLightIntensity { get; init; }
    public required TimeSpan OptimalLightPeriod { get; init; }
    
}