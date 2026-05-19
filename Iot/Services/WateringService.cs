using Entities.Plant;
using RepositoryContracts;
using ServiceContracts;

namespace Services;

public class WateringService(IWateringRepository repository) : IWateringService
{
    public async Task Create(string plantMac, Watering watering)
    {
        await repository.Create(plantMac, watering);
        await Task.CompletedTask;
    }

    public async Task<Watering?> Get(string plantMac)
    {
        return await repository.Get(plantMac);
    }
}