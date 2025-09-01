using Microsoft.EntityFrameworkCore;
using backend.DTO;
using backend.Models;
using backend.Infrastructure;
using System.Runtime.CompilerServices;

namespace backend.Services;

public class PlacementService
{
    private readonly AppDbContext _context;

    public PlacementService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlacementBracket>> CreateAllBracketsAsync(Guid turnierId)
    {
        // Alles in einer TX, damit halbfertige Zustände vermieden werden
        using var tx = await _context.Database.BeginTransactionAsync();

        // Vorbedingungen prüfen – aussagekräftige Fehler liefern
        var groups = await _context.Groups
            .Include(g => g.Teams)
            .Where(g => g.TurnierId == turnierId)
            .OrderBy(g => g.Name)
            .ToListAsync();

        if (groups.Count != 8)
            throw new InvalidOperationException($"Erwarte 8 Gruppen für Turnier {turnierId}, gefunden: {groups.Count}.");

        foreach (var g in groups)
        {
            if (g.Teams.Count < 5)
                throw new InvalidOperationException($"Gruppe {g.Name} hat nur {g.Teams.Count} Teams (benötigt 5).");
        }

        // Alte Brackets (inkl. Participants & Matches) für dieses Turnier entfernen (idempotent)
        var oldBrackets = await _context.PlacementBrackets
            .Where(b => b.TurnierId == turnierId)
            .ToListAsync();
        _context.PlacementBrackets.RemoveRange(oldBrackets);
        await _context.SaveChangesAsync();

        var result = new List<PlacementBracket>();

        for (int rank = 1; rank <= 5; rank++)
        {
            int placeMin = (rank - 1) * 8 + 1;
            int placeMax = placeMin + 7;
            var bracket = await CreateBracketForRankAsync(turnierId, rank, placeMin, placeMax);
            result.Add(bracket);
        }

        await tx.CommitAsync();
        return result;
    }

    private async Task<PlacementBracket> CreateBracketForRankAsync(Guid turnierId, int rankFromGroup, int placeMin, int placeMax)
    {
        // Teams des gewünschten Rangs (A..H) holen
        var teams = await GetTeamsForRankAsync(turnierId, rankFromGroup);
        if (teams.Count != 8)
            throw new InvalidOperationException($"Erwarte 8 Teams für Rang {rankFromGroup}, gefunden: {teams.Count}.");

        var bracket = new PlacementBracket
        {
            TurnierId = turnierId,
            RankFromGroup = rankFromGroup,
            PlaceMin = placeMin,
            PlaceMax = placeMax,
            Name = $"Plätze {placeMin}-{placeMax} ({rankFromGroup}. Plätze)"
        };

        int seed = 1;
        foreach (var t in teams)
        {
            bracket.Participants.Add(new PlacementBracketTeam
            {
                PlacementBracketId = bracket.Id,
                TeamId = t.Id,
                Seed = seed++
            });
        }

        _context.PlacementBrackets.Add(bracket);
        await _context.SaveChangesAsync();
        return bracket;
    }

    private async Task<List<Team>> GetTeamsForRankAsync(Guid turnierId, int rankFromGroup)
    {
        var groups = await _context.Groups
            .Include(g => g.Teams)
            .Where(g => g.TurnierId == turnierId)
            .OrderBy(g => g.Name) // A..H
            .ToListAsync();

        var result = new List<Team>();

        foreach (var g in groups)
        {
            // Punkte sortieren (Head-to-Head kannst du später ergänzen)
            var ordered = g.Teams
                .OrderByDescending(t => t.Points)   // ggf. Nulls → 0
                .ToList();

            if (ordered.Count >= rankFromGroup)
                result.Add(ordered[rankFromGroup - 1]);
        }

        return result;
    }

    public async Task<PlacementBracket?> GetBracketWithTeamsAndMatchesAsync(Guid bracketId)
    {
        // VARIANTE B – Bracket hat Participants (Join-Entity) -> Team
        var bracketB = await _context.PlacementBrackets
            .Include(b => b.Participants)
                .ThenInclude(p => p.Team)
            .FirstOrDefaultAsync(b => b.Id == bracketId);

        return bracketB;
    }

    public async Task<List<FinalMatch>> CreateInitialMatchesAsync(Guid bracketId, List<(Guid teamA, Guid teamB)> pairs)
    {
        // optional aber empfohlen: in einer Transaktion
        using var tx = await _context.Database.BeginTransactionAsync();

        var bracket = await _context.PlacementBrackets
            .Include(b => b.Participants).ThenInclude(p => p.Team)
            .FirstOrDefaultAsync(b => b.Id == bracketId);
        if (bracket == null)
            throw new InvalidOperationException("Bracket not found");

        if (pairs == null || pairs.Count != 4)
            throw new ArgumentException("Es müssen genau 4 Paarungen übergeben werden.");

        var allTeamIds = pairs.SelectMany(p => new[] { p.teamA, p.teamB }).ToList();
        if (allTeamIds.Count != allTeamIds.Distinct().Count())
            throw new ArgumentException("Ein Team wurde mehrfach ausgewählt.");

        // --- Runde 1 anlegen UND SPEICHERN ---
        var r1 = new List<FinalMatch>();
        int idx = 1;
        foreach (var (teamA, teamB) in pairs)
        {
            r1.Add(new FinalMatch
            {
                PlacementBracketId = bracketId,
                RoundNumber = 1,
                IndexInRound = idx++,
                TeamAId = teamA,
                TeamBId = teamB
            });
        }

        _context.FinalMatches.AddRange(r1);
        await _context.SaveChangesAsync(); // ⬅️ WICHTIG: Jetzt sind die R1-Matches in der DB

        // --- Runde 2-Platzhalter mit Quellen anlegen ---
        // Reihenfolge r1[0..3] ist jetzt stabil, IDs sind persistiert
        var r2 = new List<FinalMatch>
    {
        new() {
            PlacementBracketId = bracketId, RoundNumber = 2, IndexInRound = 1,
            SourceA_MatchId = r1[0].Id, SourceA_Take = MatchLinkType.Winner,
            SourceB_MatchId = r1[1].Id, SourceB_Take = MatchLinkType.Winner
        },
        new() {
            PlacementBracketId = bracketId, RoundNumber = 2, IndexInRound = 2,
            SourceA_MatchId = r1[2].Id, SourceA_Take = MatchLinkType.Winner,
            SourceB_MatchId = r1[3].Id, SourceB_Take = MatchLinkType.Winner
        },
        new() {
            PlacementBracketId = bracketId, RoundNumber = 2, IndexInRound = 3,
            SourceA_MatchId = r1[0].Id, SourceA_Take = MatchLinkType.Loser,
            SourceB_MatchId = r1[1].Id, SourceB_Take = MatchLinkType.Loser
        },
        new() {
            PlacementBracketId = bracketId, RoundNumber = 2, IndexInRound = 4,
            SourceA_MatchId = r1[2].Id, SourceA_Take = MatchLinkType.Loser,
            SourceB_MatchId = r1[3].Id, SourceB_Take = MatchLinkType.Loser
        },
    };

        _context.FinalMatches.AddRange(r2);
        await _context.SaveChangesAsync();

        await tx.CommitAsync();

        return r1.Concat(r2).ToList();
    }

    public async Task<FinalMatch> SetWinnerAsync(Guid matchId, Guid winnerId)
    {
        var match = await _context.FinalMatches
            .FirstOrDefaultAsync(m => m.Id == matchId);
        if (match == null)
            throw new KeyNotFoundException($"Match {matchId} nicht gefunden.");

        if (match.TeamAId != winnerId && match.TeamBId != winnerId)
            throw new ArgumentException("WinnerId gehört nicht zu diesem Match.");

        match.WinnerId = winnerId;

        var team = await _context.Teams.FindAsync(winnerId);
        if (team != null) team.Points += 1;

        await _context.SaveChangesAsync();

        await ResolveDependentMatchesAsync(match.PlacementBracketId);
        return match;
    }

    private async Task ResolveDependentMatchesAsync(Guid bracketId)
    {
        var all = await _context.FinalMatches
            .Where(m => m.PlacementBracketId == bracketId)
            .ToListAsync();

        Guid? FromSource(Guid? sourceMatchId, MatchLinkType take)
        {
            var src = all.FirstOrDefault(m => m.Id == sourceMatchId);
            if (src == null || src.WinnerId == null) return null;
            if (take == MatchLinkType.Winner) return src.WinnerId;
            if (src.TeamAId == null || src.TeamBId == null) return null;
            return src.WinnerId == src.TeamAId ? src.TeamBId : src.TeamAId;
        }

        var r2 = all.Where(m => m.RoundNumber == 2).OrderBy(m => m.IndexInRound).ToList();
        if (r2.Count < 4)
        {
            var r1 = all.Where(m => m.RoundNumber == 1).OrderBy(m => m.IndexInRound).ToList();
            if (r1.Count == 4)
            {
                var newR2 = new List<FinalMatch>
            {
                new() { PlacementBracketId = bracketId, RoundNumber = 2, IndexInRound = 1,
                        SourceA_MatchId = r1[0].Id, SourceA_Take = MatchLinkType.Winner,
                        SourceB_MatchId = r1[1].Id, SourceB_Take = MatchLinkType.Winner },

                new() { PlacementBracketId = bracketId, RoundNumber = 2, IndexInRound = 2,
                        SourceA_MatchId = r1[2].Id, SourceA_Take = MatchLinkType.Winner,
                        SourceB_MatchId = r1[3].Id, SourceB_Take = MatchLinkType.Winner },

                new() { PlacementBracketId = bracketId, RoundNumber = 2, IndexInRound = 3,
                        SourceA_MatchId = r1[0].Id, SourceA_Take = MatchLinkType.Loser,
                        SourceB_MatchId = r1[1].Id, SourceB_Take = MatchLinkType.Loser },

                new() { PlacementBracketId = bracketId, RoundNumber = 2, IndexInRound = 4,
                        SourceA_MatchId = r1[2].Id, SourceA_Take = MatchLinkType.Loser,
                        SourceB_MatchId = r1[3].Id, SourceB_Take = MatchLinkType.Loser },
            };

                _context.FinalMatches.AddRange(newR2);
                await _context.SaveChangesAsync();

                all.AddRange(newR2);
                r2 = newR2;
            }
        }

        foreach (var m in r2)
        {
            var a = FromSource(m.SourceA_MatchId, m.SourceA_Take);
            var b = FromSource(m.SourceB_MatchId, m.SourceB_Take);
            if (a.HasValue && b.HasValue && (m.TeamAId != a.Value || m.TeamBId != b.Value))
            {
                m.TeamAId = a.Value;
                m.TeamBId = b.Value;
            }
        }

        await _context.SaveChangesAsync();

        var r3 = all.Where(m => m.RoundNumber == 3).OrderBy(m => m.IndexInRound).ToList();
        if (r3.Count < 4 && r2.Count == 4)
        {
            var newR3 = new List<FinalMatch>
        {
            // Spiel 9 (Finale 1–2): Gewinner r2(1) vs Gewinner r2(2)
            new() { PlacementBracketId = bracketId, RoundNumber = 3, IndexInRound = 1,
                    SourceA_MatchId = r2[0].Id, SourceA_Take = MatchLinkType.Winner,
                    SourceB_MatchId = r2[1].Id, SourceB_Take = MatchLinkType.Winner },

            // Spiel 10 (Platz 3–4): Gewinner r2(3) vs Gewinner r2(4)
            new() { PlacementBracketId = bracketId, RoundNumber = 3, IndexInRound = 2,
                    SourceA_MatchId = r2[2].Id, SourceA_Take = MatchLinkType.Winner,
                    SourceB_MatchId = r2[3].Id, SourceB_Take = MatchLinkType.Winner },

            // Spiel 11 (Platz 5–6): Verlierer r2(1) vs Verlierer r2(2)
            new() { PlacementBracketId = bracketId, RoundNumber = 3, IndexInRound = 3,
                    SourceA_MatchId = r2[0].Id, SourceA_Take = MatchLinkType.Loser,
                    SourceB_MatchId = r2[1].Id, SourceB_Take = MatchLinkType.Loser },

            // Spiel 12 (Platz 7–8): Verlierer r2(3) vs Verlierer r2(4)
            new() { PlacementBracketId = bracketId, RoundNumber = 3, IndexInRound = 4,
                    SourceA_MatchId = r2[2].Id, SourceA_Take = MatchLinkType.Loser,
                    SourceB_MatchId = r2[3].Id, SourceB_Take = MatchLinkType.Loser },
        };

            _context.FinalMatches.AddRange(newR3);
            await _context.SaveChangesAsync();

            all.AddRange(newR3);
            r3 = newR3;
        }

        // Teams in Runde 3 setzen, sobald möglich
        foreach (var m in r3)
        {
            var a = FromSource(m.SourceA_MatchId, m.SourceA_Take);
            var b = FromSource(m.SourceB_MatchId, m.SourceB_Take);
            if (a.HasValue && b.HasValue && (m.TeamAId != a.Value || m.TeamBId != b.Value))
            {
                m.TeamAId = a.Value;
                m.TeamBId = b.Value;
            }
        }

        await _context.SaveChangesAsync();
    }


    public async Task<List<FinalMatch>> GetInitialMatchesAsync(Guid bracketId)
    {
        return await _context.FinalMatches
            .Where(m => m.PlacementBracketId == bracketId && m.RoundNumber == 1)
            .Include(m => m.TeamA)
            .Include(m => m.TeamB)
            .OrderBy(m => m.IndexInRound)
            .ToListAsync();
    }
    
    public async Task<List<(int place, Team team)>> ComputePlacementsAsync(Guid bracketId)
{
    var bracket = await _context.PlacementBrackets.FirstOrDefaultAsync(b => b.Id == bracketId)
        ?? throw new KeyNotFoundException("Bracket not found");

    var r3 = await _context.FinalMatches
        .Where(m => m.PlacementBracketId == bracketId && m.RoundNumber == 3)
        .Include(m => m.TeamA).Include(m => m.TeamB)
        .OrderBy(m => m.IndexInRound)
        .ToListAsync();

    if (r3.Count != 4 || r3.Any(m => m.WinnerId == null || m.TeamAId == null || m.TeamBId == null))
        throw new InvalidOperationException("Platzierung noch nicht vollständig.");

    var result = new List<(int place, Team team)>();

    void AddPlaces(FinalMatch m, int offset)
    {
        var loserId = m.WinnerId == m.TeamAId ? m.TeamBId!.Value : m.TeamAId!.Value;
        var winner = m.WinnerId == m.TeamAId ? m.TeamA! : m.TeamB!;
        var loser  = m.WinnerId == m.TeamAId ? m.TeamB! : m.TeamA!;
        result.Add((bracket.PlaceMin + offset,     winner));
        result.Add((bracket.PlaceMin + offset + 1, loser));
    }

    AddPlaces(r3[0], 0); // 1–2
    AddPlaces(r3[1], 2); // 3–4
    AddPlaces(r3[2], 4); // 5–6
    AddPlaces(r3[3], 6); // 7–8

    return result.OrderBy(r => r.place).ToList();
}
}