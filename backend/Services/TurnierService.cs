using Microsoft.EntityFrameworkCore;
using backend.Infrastructure;
using backend.Models;
using System.Runtime.CompilerServices;

namespace backend.Services
{
    public class TurnierService
    {
        private readonly AppDbContext _context;

        public TurnierService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Turnier> CreateTurnierAsync(Turnier turnier)
        {
            if (string.IsNullOrWhiteSpace(turnier.Name))
            throw new ArgumentException("Turniername darf nicht leer sein.");

            _context.Turniere.Add(turnier);
            await _context.SaveChangesAsync();

            var groupNames = new[] { "A", "B", "C", "D", "E", "F", "G", "H" };

            foreach (var name in groupNames)
            {
                
               _context.Groups.Add(new Group
               {
                   Name = name,
                   TurnierId = turnier.Id
               });
                
            }

            await _context.SaveChangesAsync();
            return await _context.Turniere
                .Include(t => t.Groups)
                .FirstAsync(t => t.Id == turnier.Id);
        }
    }
}