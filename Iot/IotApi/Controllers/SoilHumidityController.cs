using Entities.Plant;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using Services;

namespace IotApi.Controllers;


[ApiController]
[Route("[controller]")]
public class SoilHumidityController (ISoilHumidityService service) : ControllerBase
{
    [HttpPost("{plantMAC}")]
    public async Task<ActionResult> Create([FromRoute] string plantMAC, [FromBody] SoilHumidity soilHumidity)
    {
        try
        {
            await service.CreateAsync(plantMAC, soilHumidity);
            return CreatedAtAction(nameof(Create), new { plantMAC }, soilHumidity);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    //TODO remove after
    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
    return Ok("You've hit the soil endpoint of Plantify");
    }
}