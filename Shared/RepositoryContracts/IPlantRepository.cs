using Entities.Plant;

namespace RepositoryContracts;

public interface IPlantRepository
{
    Task<Plant> CreateAsync(Plant plant);
    Task<IEnumerable<Plant>> GetPlantsByUsernameAsync(string username, int? numberOfReadings);
    Task<Plant> GetPlantAsync(string username, string plantMAC, int? numberOfReadings);
    Task DeleteAsync(string username, string plantMAC);
    Task UpdateAsync(Plant plant);
}