using DreamEleven.DataAccess.Abstract;
using DreamEleven.Entities;
using Microsoft.EntityFrameworkCore;

namespace DreamEleven.DataAccess.Concrete
{
    public class EfPlayerRepository : IPlayerRepository
    {
        private readonly DreamElevenDbContext _context;          // Veritabanı erişimi için DbContext

        public EfPlayerRepository(DreamElevenDbContext context)
        {
            _context = context;
        }


        public async Task<List<Player>> GetAllPlayersAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task<Player?> GetBySlugAsync(string slug)
        {
            return await _context.Players
                .Include(p => p.TeamPlayers)                    // Oyuncunun takım bağlantıları
                .ThenInclude(tp => tp.Team)                     // Her bağlantının takımı
                .ThenInclude(t => t.TeamPlayers)
                .ThenInclude(tp => tp.Player)
                .FirstOrDefaultAsync(p => p.Slug == slug);      // Slug eşleşen ilk oyuncuyu getir
        }

        public async Task AddPlayerAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }
    }
}
