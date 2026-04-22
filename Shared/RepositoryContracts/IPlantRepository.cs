using Entities.Plant;

namespace RepositoryContracts;

public interface IPlantRepository
{
    Task<Plant> CreateAsync(string username,Plant plant);
    Task<IEnumerable<Plant>> GetPlantsByUsernameAsync(string username);
    Task DeleteAsync(string username,int plantId);
    Task UpdateAsync(string username,Plant plant);
    Task<IEnumerable<Plant>> GetManyAsync(string username);
    Task<IEnumerable<Plant>> GetManyAsync();
}