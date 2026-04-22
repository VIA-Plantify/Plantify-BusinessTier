namespace Entities.Plant;

public interface IWaterIntake
{
    DateTime? PumpTime { get; set; }
    double? Volume { get; set; }
    IList<double?> PastWaterIntakeReadings { get; set; }
}