using Entities;
using Entities.Plant;
using RepositoryContracts;
using ServiceContracts;

namespace Services;

public class PlantService : IPlantService
{
    private readonly IPlantRepository _repository;
    private readonly IUserRepository _userRepository;
    
    public PlantService(IPlantRepository repository, IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<Plant> CreateAsync(string username, Plant plant)
    {
        await VerifyUserExistsAsync(username);

        if (string.IsNullOrWhiteSpace(plant.Name))
        {
            throw new ArgumentException("Plant name is required.");
        }

        try
        {
            var createdPlant = await _repository.CreateAsync(username, plant);
            return createdPlant;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Failed to create plant: {e.Message}");
        }
    }

    public async Task<IEnumerable<Plant>> GetPlantsByUsernameAsync(string username)
    {
        await VerifyUserExistsAsync(username);
        
        var fetchedPlants = await _repository.GetPlantsByUsernameAsync(username);
        
        return fetchedPlants ?? throw new InvalidOperationException("Failed to fetch plants");
    }

    public async Task<Plant> GetPlantAsync(string username, int plantId)
    {
        await VerifyUserExistsAsync(username);

        var existingPlant = await _repository.GetPlantAsync(username, plantId);
       
        if (existingPlant == null)
        {
            throw new KeyNotFoundException($"Plant with ID {plantId} not found for user {username}");
        }

        return existingPlant;    
    }

    public async Task UpdateAsync(string username, Plant plant)
    {
        await VerifyUserExistsAsync(username);

        var existingPlants = await _repository.GetPlantsByUsernameAsync(username);
        var plantToUpdate = existingPlants.FirstOrDefault(p => p.Id == plant.Id);

        if (plantToUpdate == null)
        {
            throw new KeyNotFoundException($"Plant with ID {plant.Id} not found for user {username}");
        }

        plantToUpdate.Name = plant.Name;
        plantToUpdate.OptimalTemperature = plant.OptimalTemperature;
        plantToUpdate.OptimalAirHumidity = plant.OptimalAirHumidity;
        plantToUpdate.OptimalSoilHumidity = plant.OptimalSoilHumidity;
        plantToUpdate.OptimalLightSensitivity = plant.OptimalLightSensitivity;

        try
        {
            await _repository.UpdateAsync(username,plantToUpdate);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Update failed: {e.Message}");
        }
    }

    
    public async Task DeleteAsync(string username, int plantId)
    {
        await VerifyUserExistsAsync(username);

         var existingPlant = await _repository.GetPlantAsync(username, plantId);

        await _repository.DeleteAsync(username, plantId);
    }

    

    private async Task VerifyUserExistsAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required");
        }

        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            throw new KeyNotFoundException($"User '{username}' does not exist");
        }
    }
}