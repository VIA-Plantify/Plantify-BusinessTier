namespace DTOs.Plant;

public record SensorDataDto
{
    public double Temperature { get; init; }
    public double AirHumidity { get; init; }
    public double SoilHumidity { get; init; }
    public double LightIntensity { get; init; }
}