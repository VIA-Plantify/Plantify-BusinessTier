using Entities.Plant;
using RepositoryContracts;
using ServiceContracts;

namespace IotServices;

public class WateringService(IWateringRepository repository) : IWateringService
{
    public async Task CreateAsync(string plantMac, Watering watering)
    {
        await repository.CreateAsync(plantMac, watering);
        await Task.CompletedTask;
    }

    public async Task<Watering?> GetAsync(string plantMac)
    {
        return await repository.GetAsync(plantMac);
    }

    public async Task<Watering?> GetLastWithPumpTimeAsync(string plantMac)
    {
        return await repository.GetLastWithPumpTimeAsync(plantMac);
    }
}