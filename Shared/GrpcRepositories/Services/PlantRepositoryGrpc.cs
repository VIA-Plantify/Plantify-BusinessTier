using Entities.Plant;
using RepositoryContracts;
using Grpc.Core;

namespace GrpcRepositories.Services;

public class PlantRepositoryGrpc(PlantServiceProto.PlantServiceProtoClient client) : IPlantRepository
{
    private readonly PlantServiceProto.PlantServiceProtoClient _client = client;
    
    public async Task<Plant> CreateAsync(string username, Plant plant)
    {
        var request = new CreatePlantRequest
        {
            Username = username,
            Name = plant.Name,

            OptimalTemperature = plant.OptimalTemperature,
            OptimalAirHumidity = plant.OptimalAirHumidity,
            OptimalSoilHumidity = plant.OptimalSoilHumidity,
            OptimalLightIntensity = plant.OptimalLightIntensity,
            OptimalLightPeriodSeconds = (long)plant.OptimalLightPeriod.TotalSeconds,

            TemperatureScale = (GrpcRepositories.TemperatureScale)plant.TemperatureScale
        };

        try
        {
            var response = await _client.CreateAsync(request);
            return ParsePlantResponseToEntity(response);
        }
        catch (RpcException ex)
        {
            throw new InvalidOperationException($"Failed to create plant: {ex.Status.Detail}");
        }
    }

    public async Task<IEnumerable<Plant>> GetPlantsByUsernameAsync(string username)
    { try
        {
            var response = await _client.GetPlantsByUsernameAsync(new GetPlantsByUsernameRequest
            {
                Username = username
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

    public async Task<Plant> GetPlantAsync(string username, string plantMAC)
    {
        try
        {
            var response = await _client.GetAsync(new GetPlantRequest
            {
                Username = username,
                PlantMAC = plantMAC
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
            throw new InvalidOperationException($"Plant with MAC {plantMAC} not found.");
        }
        catch (RpcException ex)
        {
            throw new InvalidOperationException($"Error deleting plant: {ex.Status.Detail}");
        }
    }

    public async Task UpdateAsync(string username, Plant plant)
    {
        try
        {
            await _client.UpdateAsync(new UpdatePlantRequest
            {
                Username = username,
                PlantMAC = plant.MAC,
                Name = plant.Name,

                OptimalTemperature = plant.OptimalTemperature,
                OptimalAirHumidity = plant.OptimalAirHumidity,
                OptimalSoilHumidity = plant.OptimalSoilHumidity,
                OptimalLightIntensity = plant.OptimalLightIntensity,
                OptimalLightPeriodSeconds = (long)plant.OptimalLightPeriod.TotalSeconds,

                TemperatureScale = (GrpcRepositories.TemperatureScale)plant.TemperatureScale
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
        if (CheckResponse(response))
        {
            throw new InvalidOperationException("Plant response is null or invalid.");
        }

        return new Plant
        {
            MAC = response.PlantMAC,
            Name = response.Name,

            OptimalTemperature = response.OptimalTemperature,
            OptimalAirHumidity = response.OptimalAirHumidity,
            OptimalSoilHumidity = response.OptimalSoilHumidity,
            OptimalLightIntensity = response.OptimalLightIntensity,
            OptimalLightPeriod = TimeSpan.FromSeconds(response.OptimalLightPeriodSeconds),

            TemperatureScale = (Entities.Plant.TemperatureScale)response.TemperatureScale,

            // Sensor state
            Temperature = new Temperature
            {
                CurrentTemperature = response.CurrentTemperature
            },
            AirHumidity = new AirHumidity
            {
                CurrentAirHumidity = response.CurrentAirHumidity
            },
            SoilHumidity = new SoilHumidity
            {
                CurrentSoilHumidity = response.CurrentSoilHumidity
            },
            LightIntensity = new LightIntensity
            {
                CurrentLightIntensity = response.CurrentLightIntensity
            }
        };
    }
    
    private bool CheckResponse(PlantResponse? response)
    {
        return response is null || string.IsNullOrWhiteSpace(response.PlantMAC); 
    }
}