using Entities.Plant;

namespace RepositoryContracts;

public interface IHumidityRepository
{
    
    Task<SoilHumidity> CreateAsync(string macAddress);
    
    Task<SoilHumidity> GetSoilHumidityAsync(string macAddress);
}