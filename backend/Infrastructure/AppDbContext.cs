using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<Turnier> Turniere { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<GroupMatch> GroupMatches { get; set; }

        public DbSet<PlacementBracket> PlacementBrackets { get; set; }

        public DbSet<FinalMatch> FinalMatches { get; set; }
        public DbSet<PlacementBracketTeam> placementBracketTeams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlacementBracket>()
                .HasMany(b => b.Participants)
                .WithOne(p => p.PlacementBracket)
                .HasForeignKey(p => p.PlacementBracketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlacementBracketTeam>()
                .HasKey(p => new { p.PlacementBracketId, p.TeamId }); // Composite PK

            modelBuilder.Entity<PlacementBracketTeam>()
        .HasIndex(p => new { p.PlacementBracketId, p.Seed })
                .IsUnique(); // Seed 1..8 darf je Bracket nur einmal vergeben sein
                

            modelBuilder.Entity<FinalMatch>()
            .HasOne(m => m.PlacementBracket)
            .WithMany() // oder .WithMany(b => b.Matches) falls du Matches am Bracket hast
            .HasForeignKey(m => m.PlacementBracketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FinalMatch>()
            .HasOne(m => m.TeamA)
            .WithMany()
            .HasForeignKey(m => m.TeamAId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FinalMatch>()
            .HasOne(m => m.TeamB)
            .WithMany()
            .HasForeignKey(m => m.TeamBId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}