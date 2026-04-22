namespace Entities.Plant;

public interface IWaterLevel
{ 
    double? WaterDistance { get; set; } 
    int? WaterLevelPercentage { get; set; }
    double? CurrentWaterLevel { get; set; }
}