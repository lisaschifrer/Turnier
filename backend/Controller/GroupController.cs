using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using System.Runtime.InteropServices;
using backend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using backend.DTO;

namespace backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly GroupService _service;

    public GroupController(AppDbContext context,
                            GroupService service)
    {
        _context = context;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        return Ok(await _context.Groups.OrderBy(g => g.Name)
                    .ToListAsync());
    }

    [HttpPost("{groupId}/teams")]
    public async Task<IActionResult> AddTeam([FromRoute] Guid groupId, [FromBody] Team team)
    {
        team.GroupId = groupId;
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        return Ok(team);
    }

    [HttpGet("{groupId}/standings")]
    public async Task<ActionResult<List<TeamsStandingDto>>> GetStandings(Guid groupId)
    {
        var list = await _service.GetGroupStandingsAsync(groupId);
        return Ok(list);
    }
}