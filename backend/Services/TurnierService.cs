using Microsoft.EntityFrameworkCore;
using backend.Infrastructure;
using backend.Models;

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
            return turnier;
        }
    }
}