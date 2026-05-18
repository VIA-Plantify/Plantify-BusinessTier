using Entities.Plant;

namespace RepositoryContracts;

public interface IWateringRepository
{
    Task Create(string plantMac, Watering watering);
    Task<Watering?> Get(string plantMac );
}