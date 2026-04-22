using Entities.Plant;

namespace RepositoryContracts;

public interface IPlantRepository
{
    Task<Plant> CreateAsync(Plant plant);
    Task<Plant> GetPlantByUsernameAsync(string username);
    Task DeleteAsync(int id);
    Task UpdateAsync(Plant plant);
    Task<IEnumerable<Plant>> GetManyAsync();
}