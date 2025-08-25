public class GroupMatch
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid GroupId { get; set; }

    public Guid TeamAId { get; set; }

    public Guid TeamBId { get; set; }

    public Guid? WinnerId { get; set; }
}