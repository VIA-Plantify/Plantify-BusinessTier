using Entities.Plant;
using RepositoryContracts;
using ServiceContracts;

namespace Services;

public class SensorService(ISensorRepository _repository) : ISensorService
{
    public async Task CreateSensorData(SensorData sensorData)
    {
        await _repository.CreateSensorData(sensorData);
        await Task.CompletedTask;
    }

    public async Task<SensorData?> GetLatestAsync(string plantMac)
    {
        return await _repository.GetLatestAsync(plantMac);
    }
}