using Entities.Plant;

namespace RepositoryContracts;

public interface ISensorRepository
{
    Task CreateSensorData(SensorData sensorData);
    Task<SensorData?> GetLatestAsync(string plantMac);
}