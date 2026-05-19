namespace DTOs.Plant;

public record WateringDto
{
    public int? PumpTimeInSeconds { get; init; }
    public double? WaterLevel { get; init; }
    public DateTime? LastWaterTime { get; init; }
}