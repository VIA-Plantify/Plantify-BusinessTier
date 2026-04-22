namespace Entities.Plant;

public class SoilHumidity : IHumidity
{
    private double? currentHumidity;
    private IList<double?> pastHumidityReadings;

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

    public IList<double?> PastSoilHumidityReadings
    {
        get => pastHumidityReadings;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(PastSoilHumidityReadings), "Past humidity readings is null");
            }
            pastHumidityReadings = value;
        }
    }
    public double OptimalSoilHumidity { get; set; }
}