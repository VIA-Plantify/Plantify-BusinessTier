using Entities.Plant;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using RepositoryContracts;

namespace GrpcRepositories.Services;

public class WateringRepositoryGrpc(WateringServiceProto.WateringServiceProtoClient client) : IWateringRepository
{
    private readonly WateringServiceProto.WateringServiceProtoClient _client = client;
    
    public async Task CreateAsync(string plantMac, Watering watering)
    {
        try
        {
            if (watering.PumpTimeInSeconds == null || watering.WaterLevel == null || watering.LastWaterTime == null)
            {
                throw new ArgumentException("Watering data is incomplete.");
            }
            await _client.CreateAsync(new CreateWateringRequest
            {
                PlantMAC = plantMac,
                PumpTimeInSeconds = watering.PumpTimeInSeconds.Value,
                WaterLevel = watering.WaterLevel.Value,
                LastWaterTime = Timestamp.FromDateTime(watering.LastWaterTime.Value)
            });
        }
        catch (RpcException ex)
        {
            throw new InvalidOperationException($"Failed to log watering record: {ex.Status.Detail}");
        }    }

    public async Task<Watering?> GetAsync(string plantMac)
    {
        try
        {
            var response = await _client.GetLatestAsync(new GetLatestWateringDataRequest
            {
                PlantMAC = plantMac
            });

            return ProtoUtils.ParseWateringResponseToEntity(response);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            throw new InvalidOperationException($"Watering with MAC {plantMac} not found."); 
        }
        catch (RpcException ex)
        {
            throw new InvalidOperationException($"Error retrieving watering: {ex.Status.Detail}");
        }
    }

    public async Task<Watering?> GetLastWithPumpTimeAsync(string plantMac)
    {
        try
        {
            var response = await _client.GetLatestWithPumpTimeAsync(new GetLatestWateringDataRequest
            {
                PlantMAC = plantMac
            });

            return ProtoUtils.ParseWateringResponseToEntity(response);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            throw new InvalidOperationException($"Watering with MAC {plantMac} not found."); 
        }
        catch (RpcException ex)
        {
            throw new InvalidOperationException($"Error retrieving watering: {ex.Status.Detail}");
        }
    }
}