using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Turnier
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public List<Group> Groups { get; set; } = new();
    }
}