using backend.DTO;
using backend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class GroupService
{
    private readonly AppDbContext _context;

    public GroupService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeamsStandingDto>> GetGroupStandingsAsync(Guid groupId)
    {
        var teams = await _context.Teams
            .Where(t => t.GroupId == groupId)
            .OrderByDescending(t => t.Points)
            .ThenBy(t => t.Name)
            .ToListAsync();

        var result = new List<TeamsStandingDto>();
        int rank = 0;
        int? prevPoints = null;

        foreach (var t in teams)
        {
            if (prevPoints == null || t.Points != prevPoints.Value)
            {
                rank = result.Count + 1;
                prevPoints = t.Points;
            }

            result.Add(new TeamsStandingDto
            {
                TeamId = t.Id,
                TeamName = t.Name,
                Points = t.Points,
                Rank = rank
            });
        }
        return result;
    }
}