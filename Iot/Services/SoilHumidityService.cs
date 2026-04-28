using Entities.Plant;
using RepositoryContracts;
using ServiceContracts;

namespace Services;

public class SoilHumidityService : ISoilHumidityService
{
    private readonly ISoilHumidityRepository _repository;

    public SoilHumidityService(ISoilHumidityRepository repository)
    {
        this._repository = repository;
    }
    public async Task CreateAsync(string plantMAC, SoilHumidity soilHumidity)
    {
        if (string.IsNullOrWhiteSpace(plantMAC))
        {
            throw new ArgumentException("Plant MAC address must be provided.", nameof(plantMAC));
        }

        if (soilHumidity == null)
        {
            throw new ArgumentNullException(nameof(soilHumidity), "SoilHumidity object cannot be null.");
        }

        try
        {
            await _repository.CreateAsync(plantMAC, soilHumidity);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error persisting soil humidity for {plantMAC}: {ex.Message}", ex);
        }
    }
}