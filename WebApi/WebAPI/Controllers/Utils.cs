using DTOs.Plant;
using Entities.Plant;

namespace WebAPI.Controllers;

public static class Utils
{
    public static PlantDto PlantToDto(Plant plant)
    {
        if (plant == null)
            throw new ArgumentNullException(nameof(plant));
        if (plant.Scale == TemperatureScale.F)
        {
            plant = ConvertTemperatureToF(plant);
        }
        return new PlantDto
        {
            MAC = plant.MAC,
            Name = plant.Name,
            Username = plant.Username,

            OptimalTemperature = plant.OptimalTemperature,
            OptimalAirHumidity = plant.OptimalAirHumidity,
            OptimalSoilHumidity = plant.OptimalSoilHumidity,
            OptimalLightIntensity = plant.OptimalLightIntensity,
            Scale = plant.Scale,

            SensorData = ToSensorDto(plant.SensorData),
            Watering = ToWateringDto(plant.Watering),
            AddedDate = plant.AddedDate,
            ShouldPredictOptimal = plant.ShouldPredictOptimal,
            

            PreviousSensorData = plant.PreviousSensorData?
                .Select(ToSensorDto)
                .ToList() ?? new List<SensorDataDto>(),

            PreviousWaterings = plant.PreviousWaterings?
                .Select(ToWateringDto)
                .ToList() ?? new List<WateringDto>()
            
        };
    }

    public static SensorDataDto ToSensorDto(SensorData sensor)
    {
        if (sensor == null)
            return new SensorDataDto();

        return new SensorDataDto
        {
            Temperature = sensor.Temperature,
            AirHumidity = sensor.AirHumidity,
            SoilHumidity = sensor.SoilHumidity,
            LightIntensity = sensor.LightIntensity
        };
    }

    public static WateringDto ToWateringDto(Watering watering)
    {
        if (watering == null)
            return new WateringDto();

        return new WateringDto
        {
            PumpTimeInSeconds = watering.PumpTimeInSeconds,
            WaterLevel = watering.WaterLevel,
            LastWaterTime = watering.LastWaterTime,
        };
    }

    public static Plant ConvertTemperatureToF(Plant plant)
    {
        if (plant == null)
            throw new ArgumentNullException(nameof(plant));
        if (plant.Scale == TemperatureScale.F)
        {
            plant.OptimalTemperature = FromCToF(plant.OptimalTemperature);
            plant.SensorData.Temperature = FromCToF(plant.SensorData.Temperature);
            foreach (var sensor in plant.PreviousSensorData)
            {
                sensor.Temperature =  FromCToF(sensor.Temperature);
            }
        }

        return plant;
    }

    private static double FromCToF(double c)
    {
        return c * 1.8 + 32;
    }
}