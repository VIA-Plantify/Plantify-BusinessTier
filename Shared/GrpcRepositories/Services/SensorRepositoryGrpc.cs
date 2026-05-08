using Entities.Plant;
using RepositoryContracts;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcRepositories.Services;

public class SensorRepositoryGrpc(SensorServiceProto.SensorServiceProtoClient client) : ISensorRepository
{
    private readonly SensorServiceProto.SensorServiceProtoClient _client = client;
    public async Task CreateSensorData(SensorData sensorData)
    {
        try
        {
            var response = await _client.CreateAsync(new CreateSensorDataRequest
            {
                SoilHumidity = sensorData.SoilHumidity,
                Temperature = sensorData.Temperature,
                LightIntensity = sensorData.LightIntensity,
                PlantMAC = sensorData.PlantMAC,
                Timestamp = Timestamp.FromDateTime(sensorData.Timestamp)
            });
        }
        catch (RpcException e)
        {
            throw new InvalidOperationException($"Failed to create sensor data: {e.Status.Detail}");
        }
        
        await Task.CompletedTask;
    }

    public async Task<SensorData?> GetLatestAsync(string plantMac)
    {
        try
        {
            var response = await _client.GetLatestAsync(new GetLatestSensorDataRequest
            {
                PlantMac = plantMac
            });

            if (response is null)
            {
                throw new InvalidOperationException($"Failed to fetch sensor data: {plantMac}");
            }
            
            return new SensorData
            {
                SoilHumidity = response.SoilHumidity,
                Temperature = response.Temperature,
                LightIntensity = response.LightIntensity,
                PlantMAC = plantMac,
                Timestamp = response.Timestamp.ToDateTime()
            };
        }
        catch (RpcException e)
        {
            throw new InvalidOperationException($"Failed to get sensor data: {e.Status.Detail}");
        }
    }
}