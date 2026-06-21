using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HoldingsController : ControllerBase
{
    private readonly IHoldingService _service;

    public HoldingsController(IHoldingService service)
    {
        _service = service;
    }

    // GET api/holdings
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var holdings = await _service.GetAllHoldingsAsync();
        return Ok(holdings);
    }

    // POST api/holdings
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateHoldingDto dto)
    {
        try
        {
            var holding = await _service.AddHoldingAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = holding.Id }, holding);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    // DELETE api/holdings/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteHoldingAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}