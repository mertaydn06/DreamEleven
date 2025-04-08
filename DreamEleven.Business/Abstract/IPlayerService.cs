using DreamEleven.Entities;

namespace DreamEleven.Business.Abstract
{
    public interface IPlayerService
    {
        Task<List<Player>> GetAllPlayersAsync();             // Tüm futbolcuları getir
        Task<Player?> GetPlayerByIdAsync(int id);            // Belirli ID'ye göre futbolcu getir
        Task<Player?> GetBySlugAsync(string slug);           // Futbolcunun olduğu takımları getir
        Task AddPlayerAsync(Player player);                  // Futbolcu ekle
    }
}
