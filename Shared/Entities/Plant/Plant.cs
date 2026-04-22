namespace Entities.Plant;

public class Plant
{ 
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public User Owner { get; set; }

    public ITemperature Temperature { get; set; } = new Temperature();
    public IAirHumidity AirHumidity { get; set; } = new AirHumidity();
    public IWaterLevel WaterLevel { get; set; } = new WaterLevel();
    public IWaterIntake WaterIntake { get; set; }  = new WaterIntake();
    //public IHumidity SoilHumidity { get; set; } = new SoilHumidity();
    //public ILightSensitivity LightSensitivity { get; set; } = new LightSensitivity();
    
    public double OptimalTemperature 
    { 
        get => Temperature.OptimalTemperature; 
        set => Temperature.OptimalTemperature = value; 
    }

    public double OptimalAirHumidity 
    { 
        get => AirHumidity.OptimalAirHumidity; 
        set => AirHumidity.OptimalAirHumidity = value; 
    } 
    // public double OptimalSoilHumidity 
    // { 
    //     get => SoilHumidity.OptimalSoilHumidity; 
    //     set => SoilHumidity.OptimalSoilHumidity = value; 
    // } 
    // public double OptimalLightSensitivity 
    // { 
    //     get => LightSensitivity .OptimalLightSensitivity; 
    //     set => LightSensitivity .OptimalLightSensitivity = value; 
    // } 
    public int? WaterLevelPercentage => WaterLevel.WaterLevelPercentage;
    public double? LastWaterVolume => WaterIntake.Volume;
}