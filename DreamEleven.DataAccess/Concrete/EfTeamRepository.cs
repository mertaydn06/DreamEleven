using DreamEleven.DataAccess.Abstract;
using DreamEleven.Entities;
using Microsoft.EntityFrameworkCore;

namespace DreamEleven.DataAccess.Concrete
{
    // Entity Framework ile ITeamRepository'yi implement eden sınıf
    public class EfTeamRepository : ITeamRepository
    {
        private readonly DreamElevenDbContext _context;  // Veritabanı erişimi için DbContext

        public EfTeamRepository(DreamElevenDbContext context)
        {
            _context = context;
        }

        public async Task<List<Team>> GetAllTeamsAsync()
        {
            return await _context.Teams
                .Include(t => t.TeamPlayers)               // Takım oyuncularını dahil et
                    .ThenInclude(tp => tp.Player)          // Oyuncunun detaylarını da getir
                .ToListAsync();
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.TeamPlayers)
                    .ThenInclude(tp => tp.Player)
                .Include(t => t.Comments)                  // Yorumları da dahil et
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Team>> GetTeamsByUserIdAsync(string userId)
        {
            return await _context.Teams
                .Where(t => t.UserId == userId)
                .Include(t => t.TeamPlayers)
                    .ThenInclude(tp => tp.Player)
                .ToListAsync();
        }

        public async Task CreateTeamAsync(Team team)
        {
            _context.Teams.Add(team);                      // Takımı veritabanına ekle
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeamAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id); // ID ile takımı bul
            if (team != null)
            {
                _context.Teams.Remove(team);               // Sil ve kaydet
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTeamAsync(Team team)
        {
            _context.Teams.Update(team);                   // Güncelle ve kaydet
            await _context.SaveChangesAsync();
        }
    }
}
