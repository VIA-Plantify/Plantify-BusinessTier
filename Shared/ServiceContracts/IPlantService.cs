using Entities.Plant;
using RepositoryContracts;

namespace ServiceContracts;

public interface IPlantService : IPlantRepository
{
    Task ConvertTempScale(TemperatureScale temperatureScale, string plantMac, string username);
}