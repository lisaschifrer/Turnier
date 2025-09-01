using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using backend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using backend.DTO;

namespace backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class GroupMatchesController : ControllerBase
{
    private readonly GroupMatchService _service;

    public GroupMatchesController(GroupMatchService service)
    {
        _service = service;
    }

    [HttpPost("generate/{turnierId}")]
    public async Task<ActionResult<List<GroupMatchDto>>> GenerateGroupMatches(Guid turnierId)
{
    var matches = await _service.GenerateGroupMatchesAsync(turnierId);
    return Ok(matches);
}

    [HttpPut("{matchId}/winner/{winnerId}")]
    public async Task<IActionResult> SetWinner([FromRoute] Guid matchId, [FromRoute] Guid winnerId)
    {
        var match = await _service.SetWinnerAsync(matchId, winnerId);
        return Ok(match);
    }
}