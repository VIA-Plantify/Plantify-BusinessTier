using Entities.Plant;
using RepositoryContracts;
using Grpc.Core;

namespace GrpcRepositories.Services;

public class PlantRepositoryGrpc(PlantServiceProto.PlantServiceProtoClient client) : IPlantRepository
{
    private readonly PlantServiceProto.PlantServiceProtoClient _client = client;

    public async Task<Plant> CreateAsync(Plant plant)
    {
        try
        {
            await GetPlantAsync(plant.Username, plant.MAC, null, null);
        }
        catch (InvalidOperationException)
        {
            try
            {
                var response = await _client.CreateAsync(new CreatePlantRequest
                {
                    Username = plant.Username,
                    Name = plant.Name,
                    MAC = plant.MAC,
                    OptimalTemperature = plant.OptimalTemperature,
                    OptimalAirHumidity = plant.OptimalAirHumidity,
                    OptimalSoilHumidity = plant.OptimalSoilHumidity,
                    OptimalLightIntensity = plant.OptimalLightIntensity,
                    TemperatureScale = (GrpcRepositories.TemperatureScale)plant.Scale
                });

                return ParsePlantResponseToEntity(response);
            }
            catch (RpcException ex)
            {
                throw new InvalidOperationException($"Failed to create plant: {ex.Status.Detail}");
            }
        }

        throw new InvalidOperationException($"Plant with MAC {plant.MAC} already exists.");
    }
   

    public async Task<IEnumerable<Plant>> GetPlantsByUsernameAsync(string username, int? numberOfSensorReadings, int? numberOfWateringReadings)
    {
        try
        {
            numberOfSensorReadings ??= 0;
            numberOfWateringReadings ??= 0;
            var response = await _client.GetPlantsByUsernameAsync(new GetPlantsByUsernameRequest
            {
                Username = username,
                NumberOfReadings = numberOfSensorReadings.Value,
                NumberOfWateringReadings = numberOfWateringReadings.Value,
            });

            return response.Plants.Select(ParsePlantResponseToEntity);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            throw new InvalidOperationException($"No plants found for user {username}.");
        }
        catch (RpcException ex)
        {
            throw new InvalidOperationException($"Error retrieving plants: {ex.Status.Detail}");
        }
    }

    public async Task<Plant> GetPlantAsync(string username, string plantMAC, int? numberOfSensorReadings, int? numberOfWateringReadings)
    {
        try
        {
            numberOfWateringReadings ??= 0;
            numberOfSensorReadings ??= 0;
            var response = await _client.GetAsync(new GetPlantRequest
            {
                Username = username,
                PlantMAC = plantMAC,
                NumberOfSensorReadings = numberOfSensorReadings.Value,
                NumberOfWateringReadings =  numberOfWateringReadings.Value
            });

            return ParsePlantResponseToEntity(response);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            throw new InvalidOperationException($"Plant with MAC {plantMAC} not found."); 
        }
        catch (RpcException ex)
        {
            throw new InvalidOperationException($"Error retrieving plant: {ex.Status.Detail}");
        }
    }

    public async Task DeleteAsync(string username, string plantMAC)
    {
        try
        {
            await _client.DeleteAsync(new DeletePlantRequest
            {
                Username = username,
                PlantMAC = plantMAC
            });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // Test expects InvalidOperationException
            throw new InvalidOperationException($"Plant with MAC {plantMAC} not found.");
        }
        catch (RpcException ex)
        {
            throw new InvalidOperationException($"Error deleting plant: {ex.Status.Detail}");
        }
    }

    public async Task UpdateAsync(Plant plant)
    {
        try
        {
            await _client.UpdateAsync(new UpdatePlantRequest
            {
                Username = plant.Username,
                PlantMAC = plant.MAC,
                Name = plant.Name,
                OptimalTemperature = plant.OptimalTemperature,
                OptimalAirHumidity = plant.OptimalAirHumidity,
                OptimalSoilHumidity = plant.OptimalSoilHumidity,
                OptimalLightIntensity = plant.OptimalLightIntensity,
                TemperatureScale = (GrpcRepositories.TemperatureScale)plant.Scale
            });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            throw new InvalidOperationException($"Plant with MAC {plant.MAC} not found.");
        }
        catch (RpcException ex)
        {
            throw new InvalidOperationException($"Error updating plant: {ex.Status.Detail}");
        }
    }

    private Plant ParsePlantResponseToEntity(PlantResponse? response)
    {
        if (IsInvalidResponse(response))
            throw new InvalidOperationException("Plant response is null or invalid.");

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

            SensorData = response.SensorData == null ? new SensorData() : new SensorData
            {
                Temperature = response.SensorData.Temperature,
                AirHumidity = response.SensorData.AirHumidity,
                SoilHumidity = response.SensorData.SoilHumidity,
                LightIntensity = response.SensorData.LightIntensity,
            },

            Watering = response.Watering == null ? new Watering() : new Watering
            {
                WaterLevel = response.Watering.WaterLevel,
                PumpTimeInSeconds = response.Watering.PumpTimeInSeconds,
                LastWaterTime = response.Watering.LastWaterTime?.ToDateTime() ?? default,
                PredictedFutureWaterTime = response.Watering.PredictedFutureWaterTime?.ToDateTime() ?? default,
            }
        };
    }

    private bool IsInvalidResponse(PlantResponse? response)
    {
        return response is null || string.IsNullOrWhiteSpace(response.PlantMAC);
    }
}