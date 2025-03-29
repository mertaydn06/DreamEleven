using DreamEleven.DataAccess.Abstract;
using DreamEleven.Entities;
using Microsoft.EntityFrameworkCore;

namespace DreamEleven.DataAccess.Concrete
{
    public class EfPlayerRepository : IPlayerRepository
    {
        private readonly DreamElevenDbContext _context;

        public EfPlayerRepository(DreamElevenDbContext context)
        {
            _context = context;
        }

        public async Task<List<Player>> GetAllPlayersAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<List<Player>> GetPlayersByPositionAsync(string position)
        {
            return await _context.Players
                .Where(p => p.Position.ToString().ToLower() == position.ToLower())
                .ToListAsync();
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task<Player?> GetPlayerBySlugAsync(string slug)
        {
            return await _context.Players.FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task AddPlayerAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }
    }
}
