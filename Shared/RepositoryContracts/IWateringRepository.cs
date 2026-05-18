using Entities.Plant;

namespace RepositoryContracts;

public interface IWateringRepository
{
    Task Create(Watering watering);
    Task<Watering?> Get(string plantMac );
}