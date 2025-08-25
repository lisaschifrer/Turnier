using Microsoft.EntityFrameworkCore;
using backend.Infrastructure;
using backend.Models;
using backend.Migrations;
using System.Text.RegularExpressions;

namespace backend.Services
{
    public class GroupMatchService
    {
        private readonly AppDbContext _context;

        public GroupMatchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupMatch>> GenerateGroupMatchesAsync(Guid turnierId)
        {
            var groups = await _context.Groups
                .Include(g => g.Teams)
                .Where(g => g.TurnierId == turnierId)
                .ToListAsync();

            var matches = new List<GroupMatch>();

            foreach (var group in groups)
            {
                var teams = group.Teams;

                for (int i = 0; i < teams.Count; i++)
                {
                    for (int j = i + 1; j < teams.Count; j++)
                    {
                        var match = new GroupMatch
                        {
                            GroupId = group.Id,
                            TeamAId = teams[i].Id,
                            TeamBId = teams[j].Id
                        };
                        matches.Add(match);
                        _context.GroupMatches.Add(match);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return matches;
        }

        public async Task<GroupMatch> SetWinnerAsync(Guid matchId, Guid winnerId)
        {
            var match = await _context.GroupMatches
                .Include(m => m.TeamAId)
                .Include(m => m.TeamBId)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null) throw new Exception("Match not found");

            match.WinnerId = winnerId;

            var winner = await _context.Teams.FindAsync(winnerId);
            if (winner != null)
            {
                winner.Points += 1;
            }

            await _context.SaveChangesAsync();
            return match;
        }
    }
}