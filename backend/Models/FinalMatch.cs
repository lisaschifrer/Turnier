namespace backend.Models;

public enum MatchLinkType { None = 0, Winner = 1, Loser = 2 }
public class FinalMatch
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int RoundNumber { get; set; } = 1;   // wir starten mit Runde 1
    public int IndexInRound { get; set; }

    public Guid GroupId { get; set; }

    public Guid? TeamAId { get; set; }
    public Team? TeamA { get; set; } = null!;

    public Guid? TeamBId { get; set; }
    public Team? TeamB { get; set; } = null!;


    public Guid? WinnerId { get; set; }

    public Guid PlacementBracketId { get; set; }
    public PlacementBracket PlacementBracket { get; set; } = null!;

    // Quellen (optional; nur für Runde 2 gesetzt)
    public Guid? SourceA_MatchId { get; set; }
    public MatchLinkType SourceA_Take { get; set; } = MatchLinkType.None;

    public Guid? SourceB_MatchId { get; set; }
    public MatchLinkType SourceB_Take { get; set; } = MatchLinkType.None;
}