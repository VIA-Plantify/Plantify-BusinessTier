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
            await GetPlantAsync(plant.Username, plant.MAC, null);
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
                    OptimalLightPeriodSeconds = (long)plant.OptimalLightPeriod.TotalSeconds,

                    TemperatureScale = (GrpcRepositories.TemperatureScale)plant.TemperatureScale
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

    public async Task<IEnumerable<Plant>> GetPlantsByUsernameAsync(string username, int? numberOfReadings)
    { try
        {
            numberOfReadings = numberOfReadings ?? 0;
             var response = await _client.GetPlantsByUsernameAsync(new GetPlantsByUsernameRequest
            {
                Username = username,
                NumberOfReadings = numberOfReadings.Value
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

    public async Task<Plant> GetPlantAsync(string username, string plantMAC, int? numberOfReadings)
    {
        try
        {
            numberOfReadings = numberOfReadings ?? 0;
            var response = await _client.GetAsync(new GetPlantRequest
            {
                Username = username,
                PlantMAC = plantMAC,
                NumberOfReadings = numberOfReadings.Value
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
            Username = response.Username,
            OptimalTemperature = response.OptimalTemperature,
            OptimalAirHumidity = response.OptimalAirHumidity,
            OptimalSoilHumidity = response.OptimalSoilHumidity,
            OptimalLightIntensity = response.OptimalLightIntensity,
            OptimalLightPeriod = TimeSpan.FromSeconds(response.OptimalLightPeriodSeconds),

            TemperatureScale = (Entities.Plant.TemperatureScale)response.TemperatureScale,

            // Sensor state
            Temperature = new Temperature
            {
                Value = response.CurrentTemperature.Value,
                PastReadings = response.CurrentTemperature.PreviousValuesList
                    .Select(v => (double?)v)
                    .ToList()
            },
            AirHumidity = new AirHumidity
            {
                Value = response.CurrentAirHumidity.Value,
                PastReadings = response.CurrentAirHumidity.PreviousValuesList
                .Select(v => (double?)v)
                .ToList()
            },
            SoilHumidity = new SoilHumidity
            {
                Value = response.CurrentSoilHumidity.Value,
                PastReadings = response.CurrentSoilHumidity.PreviousValuesList
                .Select(v => (double?)v)
                .ToList()
            },
            LightIntensity = new LightIntensity
            {
                Value = response.CurrentLightIntensity.Value,
                PastReadings = response.CurrentLightIntensity.PreviousValuesList
                .Select(v => (double?)v)
                .ToList()
            }
        };
    }
    
    private bool CheckResponse(PlantResponse? response)
    {
        return response is null || string.IsNullOrWhiteSpace(response.PlantMAC); 
    }
}