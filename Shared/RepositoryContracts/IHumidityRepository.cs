using Entities.Plant;

namespace RepositoryContracts;

public interface IHumidityRepository
{
    
    Task CreateAsync(string plantMAC, SoilHumidity soilHumidity);
}