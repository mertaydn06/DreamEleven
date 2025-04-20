using DreamEleven.DataAccess.Abstract;
using DreamEleven.Entities;
using Microsoft.EntityFrameworkCore;

namespace DreamEleven.DataAccess.Concrete
{
    public class EfTeamRepository : ITeamRepository
    {
        private readonly DreamElevenDbContext _context;          // Veritabanı erişimi için DbContext

        public EfTeamRepository(DreamElevenDbContext context)
        {
            _context = context;
        }


        public async Task<List<Team>> GetAllTeamsAsync()
        {
            return await _context.Teams
                .Include(t => t.TeamPlayers)                     // Takım oyuncularını dahil et
                .ThenInclude(tp => tp.Player)                    // Oyuncunun detaylarını da getir
                .ToListAsync();
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.TeamPlayers)                    // Takımın içindeki tüm TeamPlayer (oyuncu ilişkilerini) dahil et
                .ThenInclude(tp => tp.Player)                   // TeamPlayer’dan Player’a olan ilişkiyi de yükle
                .Include(t => t.Comments)                       // Takıma yapılmış tüm yorumları da dahil et
                .FirstOrDefaultAsync(t => t.Id == id);          // Verilen ID'ye sahip ilk takımı getir (bulunamazsa null döner)
        }

        public async Task<List<Team>> GetTeamsByUserIdAsync(string userId)
        {
            return await _context.Teams
                .Where(t => t.UserId == userId)                 // Verilen userId'ye sahip olan tüm takımları filtrele
                .Include(t => t.TeamPlayers)                    // Her takımın içindeki TeamPlayer ilişkilerini dahil et
                .ThenInclude(tp => tp.Player)                   // TeamPlayer'dan Player'a olan ilişkiyi de dahil et
                .OrderByDescending(t => t.CreatedAt)            // Takımları oluşturma tarihine göre sırala
                .ToListAsync();                                 // Sonucu liste olarak asenkron şekilde getir
        }


        public async Task<List<Team>> GetTeamsByPlayerIdAsync(int playerId)
        {
            return await _context.TeamPlayers
                .Where(tp => tp.PlayerId == playerId)           // Belirli bir oyuncuya ait tüm TeamPlayer kayıtlarını filtrele
                .Select(tp => tp.Team)                          // Bu kayıtlar üzerinden ilgili takımları seç (Team nesneleri)
                .Distinct()                                     // Aynı takım birden fazla kez varsa, tekrar edenleri kaldır
                .Include(t => t.TeamPlayers)                    // Her takımın içindeki oyuncular (TeamPlayers) da yüklensin
                .ToListAsync();                                 // Sonucu liste olarak veritabanından çek
        }


        public async Task CreateTeamAsync(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeamAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTeamAsync(Team team)
        {
            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
        }

        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

    }
}
