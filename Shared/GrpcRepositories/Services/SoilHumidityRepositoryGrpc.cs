using Entities.Plant;
using Grpc.Core;
using RepositoryContracts;

namespace GrpcRepositories.Services;

public class SoilHumidityRepositoryGrpc(SoilHumidityServiceProto.SoilHumidityServiceProtoClient client) : ISoilHumidityRepository
{
    private readonly SoilHumidityServiceProto.SoilHumidityServiceProtoClient _client = client;

    public async Task CreateAsync(string plantMAC, SoilHumidity soilHumidity)
    {
        
        double valueToSend = soilHumidity.Value ?? 0.0;
        try
        {
            var request = new CreateSoilHumidityRequest
            {
                PlantMAC = plantMAC,
                Value = valueToSend
            };
            
            await _client.CreateAsync(request);
        }
        catch (RpcException e)
        {
            throw new InvalidOperationException($"Failed to create soil humidity record via gRPC: {e.Status.Detail}", e);
        }
        catch (Exception e)
        {
            throw new Exception($"An unexpected error occurred: {e.Message}", e);
        }
    }
    
}
