namespace backend.DTO;

public class FinalStandingDto
{
    public int Place { get; set; }
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public int? Points { get; set; }
}