using Entities.Plant;
using RepositoryContracts;

namespace ServiceContracts;

public interface IPlantService : IPlantRepository
{ 
    Task ConvertTemperatureAsync(string plantMAC, string username, TemperatureScale scale);
}