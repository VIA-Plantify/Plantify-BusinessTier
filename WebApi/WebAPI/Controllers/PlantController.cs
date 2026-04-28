using DTOs.Plant;
using Entities.Plant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PlantController(IPlantService plantService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Plant>> CreateAsync([FromBody] PlantDto dto)
    {
        var loggedInUsername = User.FindFirst("Username")?.Value;
        if (string.IsNullOrEmpty(loggedInUsername))
        {
            return Unauthorized("User identity not found in token.");
        }
        try
        {
            var plant = new Plant()
            {
                MAC = dto.MAC,
                Name = dto.Name,
                Username = loggedInUsername,
                OptimalTemperature = dto.OptimalTemperature,
                OptimalAirHumidity = dto.OptimalAirHumidity,
                OptimalSoilHumidity = dto.OptimalSoilHumidity,
                OptimalLightIntensity = dto.OptimalLightIntensity,
                OptimalLightPeriod = dto.OptimalLightPeriod
            };
            var created = await plantService.CreateAsync(plant);
            CheckPlantDataIntegrity(created);
 
            return CreatedAtAction(
                nameof(GetPlant),
                new { plantMAC = created.MAC },
                created
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
 
    [HttpGet("p/{plantMAC}")]
    [Authorize]
    public async Task<ActionResult<Plant>> GetPlant([FromRoute] string? plantMAC, [FromQuery] int? numberOfReadings)
    {
        var loggedInUsername = User.FindFirst("Username")?.Value;
        if (string.IsNullOrEmpty(loggedInUsername))
        {
            return Unauthorized("User identity not found in token.");
        }
        try
        {
            if (plantMAC == null)
            {
                return BadRequest("Plant MAC address must be provided");
            }
            var plant = await plantService.GetPlantAsync(loggedInUsername, plantMAC, numberOfReadings);
            CheckPlantDataIntegrity(plant);
            return Ok(plant);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
 
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Plant>>> GetAllPlants([FromQuery] int? numberOfReadings)
    {
        var loggedInUsername = User.FindFirst("Username")?.Value;
        if (string.IsNullOrEmpty(loggedInUsername))
        {
            return Unauthorized("User identity not found in token.");
        }
        try
        {
            var plants = await plantService.GetPlantsByUsernameAsync(loggedInUsername, numberOfReadings);
            return Ok(plants);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, "Internal server error while fetching plants.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
 
    [HttpPut("{plantMAC}")]
    [Authorize]
    public async Task<ActionResult> Update([FromRoute] string plantMAC, [FromBody] PlantDto dto)
    {
        var loggedInUsername = User.FindFirst("Username")?.Value;
 
        if (string.IsNullOrEmpty(loggedInUsername))
        {
            return Unauthorized("User identity not found in token.");
        }
        try
        {
            var existingPlant = await plantService.GetPlantAsync(loggedInUsername, plantMAC, null);
            if (existingPlant == null)
            {
                return NotFound("Plant not found.");
            }
            var plantToUpdate = new Plant()
            {
                MAC = existingPlant.MAC,
                Name = dto.Name,
                OptimalTemperature = dto.OptimalTemperature,
                OptimalAirHumidity = dto.OptimalAirHumidity,
                OptimalSoilHumidity = dto.OptimalSoilHumidity,
                OptimalLightIntensity = dto.OptimalLightIntensity,
                OptimalLightPeriod = dto.OptimalLightPeriod
            };
            await plantService.UpdateAsync(plantToUpdate);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
 
    [HttpDelete("{plantMAC}")]
    [Authorize]
    public async Task<ActionResult> Delete([FromRoute] string plantMAC)
    {
        var loggedInUsername = User.FindFirst("Username")?.Value;
        if (string.IsNullOrEmpty(loggedInUsername))
        {
            return Unauthorized("User identity not found in token.");
        }
        try
        {
            var plant = await plantService.GetPlantAsync(loggedInUsername, plantMAC, null);
            CheckPlantDataIntegrity(plant);
            await plantService.DeleteAsync(loggedInUsername, plantMAC);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal error occurred");
        }
    }
 
    private void CheckPlantDataIntegrity(Plant? plant)
    {
        if (plant == null)
        {
            throw new KeyNotFoundException("Plant not found");
        }
        if (string.IsNullOrEmpty(plant.MAC) || string.IsNullOrEmpty(plant.Name))
        {
            throw new InvalidOperationException("Plant MAC and Name must be provided");
        }
    }
}