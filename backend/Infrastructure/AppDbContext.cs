using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {}

        public DbSet<Turnier> Turniere {get; set;}
    }
}