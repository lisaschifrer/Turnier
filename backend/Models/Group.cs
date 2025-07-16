using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public List<Team> Teams { get; set; } = new();
    }
}