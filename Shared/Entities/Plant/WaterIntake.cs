namespace Entities.Plant;

public class WaterIntake
{
    private DateTime? pumpTime;
    private double? volume;
    
    public DateTime? PumpTime
    {
        get => pumpTime;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(PumpTime), "Pump time is null");

            if (value > DateTime.Now)
                throw new ArgumentException("Pump time cannot be in the future");

            pumpTime = value;
        }
    }

    public double? Volume
    {
        get => volume;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(Volume), "Volume is null");

            if (value < 0)
                throw new ArgumentException("Volume cannot be negative");

            volume = value;
        }
    }
    public IList<double?> PastWaterIntakeReadings { get; set; } = new List<double?>();
}