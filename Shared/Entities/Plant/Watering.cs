namespace Entities.Plant;

public class Watering
{
    private int? pumpTimeInSeconds;
    private DateTime? lastWaterTime;
    private DateTime? predictedFutureWaterTime;
    private double? waterLevel;

    public int? PumpTimeInSeconds
    {
        get => pumpTimeInSeconds;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(PumpTimeInSeconds), "Pump time in seconds is null");

            if (value < 0)
                throw new ArgumentException("Pump time in seconds cannot be negative");

            pumpTimeInSeconds = value;
        }
    }

    public DateTime? LastWaterTime
    {
        get => lastWaterTime;
        set
        {
            if (value > DateTime.Now)
                throw new ArgumentException("Last water time cannot be in the future");

            lastWaterTime = value;
        }
    }

    public DateTime? PredictedFutureWaterTime
    {
        get => predictedFutureWaterTime;
        set
        {
            if (value < DateTime.Now)
                throw new ArgumentException("Predicted future water time must be in the future");

            predictedFutureWaterTime = value;
        }
    }

    public double? WaterLevel
    {
        get => waterLevel;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(WaterLevel), "Water level is null");

            if (value < 0 || value > 100)
                throw new ArgumentException("Water level must be between 0 and 100");

            waterLevel = value;
        }
    }

    public IList<double?> PastWaterLevelReadings { get; set; } = new List<double?>();
}