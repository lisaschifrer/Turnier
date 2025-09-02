namespace backend.DTO;

public class TeamsStandingDto
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public int? Points { get; set; } = 0;
    public int Rank { get; set; }

}