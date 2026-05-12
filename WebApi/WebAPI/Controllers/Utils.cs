using DTOs.Plant;
using Entities.Plant;

namespace WebAPI.Controllers;

public static class Utils
{
    public static PlantDto PlantToDto(Plant plant)
    {
        if (plant == null)
            throw new ArgumentNullException(nameof(plant));

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
            PredictedFutureWaterTime = watering.PredictedFutureWaterTime
        };
    }
}