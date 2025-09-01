namespace backend.Models;

public class PlacementBracket
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TurnierId { get; set; }
    public int RankFromGroup { get; set; }
    public int PlaceMin { get; set; }
    public int PlaceMax { get; set; }
    public string Name { get; set; } = "";

    public List<PlacementBracketTeam> Participants { get; set; } = new();


}