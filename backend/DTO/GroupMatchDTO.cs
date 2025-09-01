public class GroupMatchDto
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }

     public string GroupName { get; set; } = string.Empty;

    public Guid TeamAId { get; set; }
    public string TeamAName { get; set; } = string.Empty;

    public Guid TeamBId { get; set; }
    public string TeamBName { get; set; } = string.Empty;

    public Guid? WinnerId { get; set; }
}
