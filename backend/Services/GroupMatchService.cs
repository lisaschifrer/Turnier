using Microsoft.EntityFrameworkCore;
using backend.Infrastructure;
using backend.Models;
using backend.Migrations;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace backend.Services
{
    public class GroupMatchService
    {
        private readonly AppDbContext _context;

        public GroupMatchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupMatchDto>> GenerateGroupMatchesAsync(Guid turnierId)
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
            var matchDtos = matches.Select(m => new GroupMatchDto
            {
                Id = m.Id,
                GroupId = m.GroupId,
                GroupName = groups.First(g => g.Id == m.GroupId).Name,
                TeamAId = m.TeamAId,
                TeamBId = m.TeamBId,
                WinnerId = m.WinnerId,
                TeamAName = groups.SelectMany(g => g.Teams).First(t => t.Id == m.TeamAId).Name,
                TeamBName = groups.SelectMany(g => g.Teams).First(t => t.Id == m.TeamBId).Name
            }).ToList();

            return matchDtos;
        }

        public async Task<GroupMatch> SetWinnerAsync(Guid matchId, Guid winnerId)
        {
            var match = await _context.GroupMatches.FindAsync(matchId);
            if (match == null) throw new Exception("Match not found");

            var team = await _context.Teams.FindAsync(winnerId);
            if (team == null) throw new Exception("Winner team not found");

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