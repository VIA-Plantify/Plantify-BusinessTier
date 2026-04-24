using Entities.Plant;

namespace RepositoryContracts;

public interface IPlantRepository
{
    Task<Plant> CreateAsync(string username, Plant plant);
    Task<IEnumerable<Plant>> GetPlantsByUsernameAsync(string username);
    Task<Plant> GetPlantAsync(string username, string plantMAC);
    Task DeleteAsync(string username, string plantMAC);
    Task UpdateAsync(string username, Plant plant);
}