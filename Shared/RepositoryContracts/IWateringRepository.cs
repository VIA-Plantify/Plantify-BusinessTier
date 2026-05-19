using Entities.Plant;

namespace RepositoryContracts;

public interface IWateringRepository
{
    Task CreateAsync(string plantMac, Watering watering);
    Task<Watering?> GetAsync(string plantMac );
    Task<Watering?> GetLastWithPumpTimeAsync(string plantMac);
}