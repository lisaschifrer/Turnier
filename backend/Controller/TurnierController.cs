using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class TurnierController : ControllerBase
{
    private readonly TurnierService _service;

    public TurnierController(TurnierService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTurnier([FromBody] Turnier turnier)
    {
        try
        {
            var created = await _service.CreateTurnierAsync(turnier);
            return CreatedAtAction(nameof(CreateTurnier), new { id = created.Id }, created);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}