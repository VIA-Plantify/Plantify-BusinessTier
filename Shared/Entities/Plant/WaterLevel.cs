namespace Entities.Plant;

public class WaterLevel
{
    private double? waterDistance;
    private int? waterLevelPercentage;
    private double? currentWaterLevel;

    public double? WaterDistance
    {
        get => waterDistance;
        set
        {
            if (value is null)
                throw new ArgumentException("Water distance is null");

            if (value < 0)
                throw new ArgumentException("Water distance cannot be negative");

            waterDistance = value;
        }
    }

    public int? WaterLevelPercentage
    {
        get => waterLevelPercentage;
        set
        {
            if (value is null)
                throw new ArgumentException("Water level percentage is null");

            if (value < 0 || value > 100)
                throw new ArgumentException("Water level percentage must be between 0 and 100");

            waterLevelPercentage = value;
        }
    }

    public double? CurrentWaterLevel
    {
        get => currentWaterLevel;
        set
        {
            if (value is null)
                throw new ArgumentException("Current water level is null");

            if (value < 0)
                throw new ArgumentException("Current water level cannot be negative");

            currentWaterLevel = value;
        }
    }
}