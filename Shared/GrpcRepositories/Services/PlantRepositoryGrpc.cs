using Entities.Plant;
using Google.Protobuf.WellKnownTypes;
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
                    TemperatureScale = (GrpcRepositories.TemperatureScale)plant.Scale,
                    AddedDate = Timestamp.FromDateTime(plant.AddedDate),
                    ShouldPredictOptimal = plant.ShouldPredictOptimal,
                });

                return ProtoUtils.ParsePlantResponseToEntity(response);
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

            return response.Plants.Select(ProtoUtils.ParsePlantResponseToEntity);
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

           return ProtoUtils.ParsePlantResponseToEntity(response);
            
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
                TemperatureScale = (GrpcRepositories.TemperatureScale)plant.Scale,
                
                ShouldPredictOptimal = plant.ShouldPredictOptimal,
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

    
}
