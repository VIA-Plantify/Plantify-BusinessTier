using Entities.Plant;

namespace RepositoryContracts;

public interface IPlantRepository
{
    Task<Plant> CreateAsync(Plant plant);
    Task<IEnumerable<Plant>> GetPlantsByUsernameAsync(string username, int? numberOfSensorReadings, int? numberOfWateringReadings);
    Task<Plant> GetPlantAsync(string username, string plantMAC, int? numberOfSensorReadings, int? numberOfWateringReadings);
    Task DeleteAsync(string username, string plantMAC);
    Task UpdateAsync(Plant plant);
}