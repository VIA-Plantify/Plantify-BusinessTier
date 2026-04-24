using Entities.Plant;
using RepositoryContracts;

namespace GrpcRepositories.Services;

public class PlantRepositoryGrpc : IPlantRepository
{
    public Task<Plant> CreateAsync(string username, Plant plant)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Plant>> GetPlantsByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<Plant> GetPlantAsync(string username, string plantMAC)
    {
        throw new NotImplementedException();
    }

    public Task<Plant> GetPlantAsync(int id, string username)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string username, string plantMAC)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(string username, Plant plant)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Plant>> GetManyAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Plant>> GetManyAsync()
    {
        throw new NotImplementedException();
    }
}