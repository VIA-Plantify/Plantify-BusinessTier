using Entities.Plant;

namespace RepositoryContracts;

public interface IWateringRepository
{
    Task Create(Watering watering);
    Task<Watering?> GetWatering(string plantMAC);
}