using backend.Services;
using Microsoft.AspNetCore.Mvc;
using backend.DTO;
using backend.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class PlacementController : ControllerBase
{
    private readonly PlacementService _service;
    private readonly AppDbContext _context;

    public PlacementController(PlacementService service,
                                AppDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpPost("{turnierId}/create-all")]
    public async Task<IActionResult> CreateAll(Guid turnierId)
    {
        try
        {
            var brackets = await _service.CreateAllBracketsAsync(turnierId);
            return Ok(brackets);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{bracketId}")]
    public async Task<IActionResult> GetBracket(Guid bracketId)
    {
        var bracket = await _service.GetBracketWithTeamsAndMatchesAsync(bracketId);
        if (bracket == null) return NotFound();
        return Ok(bracket);
    }

    [HttpPost("{bracketId}/matches/initial")]
    public async Task<IActionResult> CreateInitialMatches(Guid bracketId, [FromBody] List<CreateMatchDto> pairs)
    {
        var tuplePairs = pairs.Select(p => (p.TeamAId, p.TeamBId)).ToList();
        var matches = await _service.CreateInitialMatchesAsync(bracketId, tuplePairs);
        return Ok(matches);
    }

    // GET /api/placement/{bracketId}/matches/initial
    [HttpGet("{bracketId}/matches/initial")]
    public async Task<IActionResult> GetInitialMatches(Guid bracketId)
    {
        var matches = await _service.GetInitialMatchesAsync(bracketId);
        return Ok(matches);
    }

    // POST /api/placement/matches/{matchId}/winner/{winnerId}
    [HttpPost("matches/{matchId}/winner/{winnerId}")]
    public async Task<IActionResult> SetWinner(Guid matchId, Guid winnerId)
    {
        try
        {
            var updated = await _service.SetWinnerAsync(matchId, winnerId);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{turnierId}/by-rank/{rank}")]
    public async Task<IActionResult> GetBracketByRank(Guid turnierId, int rank, [FromServices] AppDbContext _context)
    {
        var bracket = await _context.PlacementBrackets
            .Include(b => b.Participants)
            .ThenInclude(p => p.Team)
        .FirstOrDefaultAsync(b => b.TurnierId == turnierId && b.RankFromGroup == rank);

        if (bracket == null) return NotFound();
        return Ok(bracket);
    }

    [HttpGet("{bracketId}/matches")]
    public async Task<IActionResult> GetAllMatches(Guid bracketId, [FromServices] AppDbContext db)
    {
        var matches = await db.FinalMatches
            .Where(m => m.PlacementBracketId == bracketId)
            .Include(m => m.TeamA).Include(m => m.TeamB)
            .OrderBy(m => m.RoundNumber).ThenBy(m => m.IndexInRound)
            .ToListAsync();

        return Ok(matches);
    }

    [HttpGet("{bracketId}/placements")]
    public async Task<IActionResult> GetPlacements(Guid bracketId)
    {
        var list = await _service.ComputePlacementsAsync(bracketId);
        return Ok(list.Select(x => new { place = x.place, teamId = x.team.Id, teamName = x.team.Name }));
    }
    
    [HttpGet("{turnierId}/final-standings")]
public async Task<ActionResult<List<FinalStandingDto>>> GetFinalStandings(Guid turnierId)
{
    var brackets = await _context.PlacementBrackets
        .Where(b => b.TurnierId == turnierId)
        .OrderBy(b => b.PlaceMin)
        .ToListAsync();

    if (brackets.Count == 0)
        return NotFound();

    var output = new List<FinalStandingDto>();

    foreach (var b in brackets)
    {
        // nutzt deine Service-Logik: gibt (place, team) zurück
        var placements = await _service.ComputePlacementsAsync(b.Id);

        output.AddRange(placements.Select(p => new FinalStandingDto {
            Place = p.place,
            TeamId = p.team.Id,
            TeamName = p.team.Name,
            Points = p.team.Points
        }));
    }

    return Ok(output.OrderBy(x => x.Place).ToList());
}
}