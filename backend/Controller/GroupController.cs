using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using System.Runtime.InteropServices;
using backend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private readonly AppDbContext _context;

    public GroupController(AppDbContext context)
    {
        _context = context;
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
}