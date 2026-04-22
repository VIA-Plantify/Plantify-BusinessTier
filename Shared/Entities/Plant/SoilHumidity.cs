namespace Entities.Plant;

public class SoilHumidity : IHumidity
{
    private double? currentHumidity;

    public double? CurrentSoilHumidity
    {
        get => currentHumidity;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(CurrentSoilHumidity), "Soil humidity is null");
            }

            if (value < 0)
            {
                throw new ArgumentException("Soil humidity cannot be negative");
            }

            currentHumidity = value;
        }
    }

    public IList<double?> PastSoilHumidityReadings { get; set; } = new List<double?>();
    
    public double OptimalSoilHumidity { get; set; }
}