using Entities.Plant;

namespace RepositoryContracts;

public interface ISoilHumidityRepository
{
    
    Task CreateAsync(string plantMAC, SoilHumidity soilHumidity);
}