using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Team
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public int GroupId { get; set; }

        public Group Group { get; set; } = null;
    }
}