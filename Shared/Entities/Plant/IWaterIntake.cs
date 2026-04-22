namespace Entities.Plant;

public interface IWaterIntake
{
    double? PumpTime { get; set; }
    double? Volume { get; set; }
    IList<double?> PastWaterIntakeReadings { get; set; }
}