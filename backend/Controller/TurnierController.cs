using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using backend.Infrastructure;
using Microsoft.EntityFrameworkCore;

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
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("{turnierId}/diagnostics")]
public async Task<IActionResult> Diagnostics(Guid turnierId, [FromServices] AppDbContext db)
{
    var groups = await db.Groups
        .Include(g => g.Teams)
        .Where(g => g.TurnierId == turnierId)
        .OrderBy(g => g.Name)
        .ToListAsync();

    return Ok(new {
        groupCount = groups.Count,
        groups = groups.Select(g => new { g.Name, teamCount = g.Teams.Count })
    });
}
}