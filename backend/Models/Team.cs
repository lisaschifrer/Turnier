using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Team
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public Guid GroupId { get; set; }

        public int? Points { get; set; } = 0;
    }
}