namespace Plantify.WebApi.Services.Models;

public class SensorResponse
{
    public string RequestId { get; set; } = "";
    public string Mac { get; set; } = "";

    public double Temperature { get; set; }
    public double AirHumidity { get; set; }
    public int SoilHumidity { get; set; }
    public int Light { get; set; }
}