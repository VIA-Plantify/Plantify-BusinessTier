namespace Entities.Plant;

public class WaterIntake : IWaterIntake
{
    private double? pumpTime;
    private double? volume;
    private IList<double?> pastWaterIntakeReadings = new List<double?>();

    public double? PumpTime
    {
        get => pumpTime;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(PumpTime), "Pump time is null");

            if (value < 0)
                throw new ArgumentException("Pump time cannot be negative");

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

    public IList<double?> PastWaterIntakeReadings
    {
        get => pastWaterIntakeReadings;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(PastWaterIntakeReadings), "Past readings list is null");

            pastWaterIntakeReadings = value;
        }
    }
}