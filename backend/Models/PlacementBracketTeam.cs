using System.Text.Json.Serialization;

namespace backend.Models;

public class PlacementBracketTeam
{
    public Guid PlacementBracketId { get; set; }

    [JsonIgnore]
    public PlacementBracket PlacementBracket { get; set; } = null!;

    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    // Optional: Seed/Startposition im Bracket (1..8)
    public int? Seed { get; set; }
}