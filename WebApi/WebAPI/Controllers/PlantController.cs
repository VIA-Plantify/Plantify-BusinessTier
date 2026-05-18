using DTOs.Plant;
using Entities.Plant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ServiceContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PlantController(IPlantService plantService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PlantDto>> CreateAsync([FromBody] PlantDto dto)
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
                AddedDate = dto.AddedDate,
                ShouldPredictOptimal = dto.ShouldPredictOptimal,
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
    public async Task<ActionResult<PlantDto>> GetPlant([FromRoute] string? plantMAC, [FromQuery] int? numberOfSensorReadings, [FromQuery] int? numberOfWateringReadings)
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

            var plant = await plantService.GetPlantAsync(loggedInUsername, plantMAC, numberOfSensorReadings,
                numberOfWateringReadings);
            CheckPlantDataIntegrity(plant);
            return Ok(Utils.PlantToDto(plant));
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
    public async Task<ActionResult<IEnumerable<PlantDto>>> GetAllPlants([FromQuery] int? numberOfSensorReadings, [FromQuery] int? numberOfWateringReadings)
    {
         var loggedInUsername = User.FindFirst("Username")?.Value;
         if (string.IsNullOrEmpty(loggedInUsername))
         {
            return Unauthorized("User identity not found in token.");
         }
        try
        {
            var plants = await plantService.GetPlantsByUsernameAsync(loggedInUsername, numberOfSensorReadings, numberOfWateringReadings);
            List<PlantDto> plantDtos = new List<PlantDto>();
            foreach (var plant in plants)
            {
                plantDtos.Add(Utils.PlantToDto(plant));
            }
            return Ok(plantDtos);
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
            var existingPlant = await plantService.GetPlantAsync(loggedInUsername, plantMAC, null,null);
            if (existingPlant == null)
            {
                return NotFound("Plant not found.");
            }
            var plantToUpdate = new Plant()
            {
                MAC = existingPlant.MAC,
                Name = dto.Name,
                Username = loggedInUsername,
                Scale = dto.Scale,
                OptimalTemperature = dto.OptimalTemperature,
                OptimalAirHumidity = dto.OptimalAirHumidity,
                OptimalSoilHumidity = dto.OptimalSoilHumidity,
                OptimalLightIntensity = dto.OptimalLightIntensity,
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
            var plant = await plantService.GetPlantAsync(loggedInUsername, plantMAC, null, null);
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

    [HttpPost("temperature/{plantMac}")]
    public async Task<ActionResult> ConvertTemperature([FromRoute] string plantMAC, [FromQuery] TemperatureScale scale)
    {
        var loggedInUsername = User.FindFirst("Username")?.Value;
        if (string.IsNullOrEmpty(loggedInUsername))
        {
            return Unauthorized("User identity not found in token.");
        }
        try
        {
            var existingPlant = await plantService.GetPlantAsync(loggedInUsername, plantMAC, null,null);
            if (existingPlant == null)
            {
                return NotFound("Plant not found.");
            }
            
            existingPlant.Scale = scale; 
            
            
            await plantService.UpdateAsync(existingPlant);
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