namespace Entities.Plant;

public class Plant
{ 
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ITemperature Temperature { get; set; } = new Temperature();
    public IAirHumidity AirHumidity { get; set; } = new AirHumidity();
    public IWaterLevel WaterLevel { get; set; } = new WaterLevel();
    public IWaterIntake WaterIntake { get; set; }  = new WaterIntake();
    public IHumidity SoilHumidity { get; set; } = new SoilHumidity();
    public ILightIntensity LightIntensity { get; set; } = new LightIntensity();
    
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
     public double OptimalSoilHumidity 
     { 
         get => SoilHumidity.OptimalSoilHumidity; 
        set => SoilHumidity.OptimalSoilHumidity = value; 
     } 
     public double OptimalLightSensitivity 
     { 
         get => LightIntensity .OptimalLightIntensity; 
        set => LightIntensity .OptimalLightIntensity = value; 
     } 
    public int? WaterLevelPercentage => WaterLevel.WaterLevelPercentage;
    public double? LastWaterVolume => WaterIntake.Volume;
}