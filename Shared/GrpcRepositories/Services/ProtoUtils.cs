using Entities.Plant;

namespace GrpcRepositories.Services;

public static class ProtoUtils
{
    public static Plant ParsePlantResponseToEntity(PlantResponse? response)
    {
        if (IsInvalidResponse(response))
            throw new InvalidOperationException("Plant response is null or invalid.");
        List<SensorData> sensorDatas = new List<SensorData>();
        if (response?.PreviousSensorReadings is not null)
        {
            foreach (var sensor in response.PreviousSensorReadings.PreviousSensorReadings)
            {
                sensorDatas.Add(ParseSensorDataResponseToEntity(sensor));
            }
        }
        List<Watering> waterings = new List<Watering>();
        if (response?.PreviousWateringReadings is not null)
        {
           
            foreach (var watering in response.PreviousWateringReadings.Readings)
            {
                waterings.Add(ParseWateringResponseToEntity(watering));
            }
        }
        
        
        return new Plant
        {
            MAC = response.PlantMAC,
            Name = response.Name,
            Username = response.Username,
            OptimalTemperature = response.OptimalTemperature,
            OptimalAirHumidity = response.OptimalAirHumidity,
            OptimalSoilHumidity = response.OptimalSoilHumidity,
            OptimalLightIntensity = response.OptimalLightIntensity,
            Scale = (Entities.Plant.TemperatureScale)response.TemperatureScale,
            AddedDate = response.AddedDate?.ToDateTime().ToUniversalTime() ?? default,
            ShouldPredictOptimal = response.ShouldPredictOptimal,

            SensorData = ParseSensorDataResponseToEntity(response.SensorData),

            Watering = ParseWateringResponseToEntity(response.Watering),
            PreviousSensorData = sensorDatas,
            PreviousWaterings = waterings
        };
    }
    public static SensorData ParseSensorDataResponseToEntity(SensorResponse? response)
    {
        if (response is null)
        {
            return new SensorData();
        }

        return new SensorData
        {
            AirHumidity = response.AirHumidity,
            SoilHumidity = response.SoilHumidity,
            LightIntensity = response.LightIntensity,
            Temperature = response.Temperature,
            Timestamp = response.Timestamp.ToDateTime().ToUniversalTime(),
        };
    }
    public static Watering ParseWateringResponseToEntity(WateringResponse? response)
    {
        if (response is null)
        {
            return new Watering();
        }

        return new Watering
        {
            PumpTimeInSeconds = response.PumpTimeInSeconds,
            WaterLevel = response.WaterLevel,
            LastWaterTime = response.LastWaterTime?.ToDateTime().ToUniversalTime() ?? default,
        };
    }

    private static bool IsInvalidResponse(PlantResponse? response)
    {
        return response is null || string.IsNullOrWhiteSpace(response.PlantMAC);
    }
}