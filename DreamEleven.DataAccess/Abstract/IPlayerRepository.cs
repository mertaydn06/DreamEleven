using DreamEleven.Entities;

namespace DreamEleven.DataAccess.Abstract
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllPlayersAsync();                 // Tüm oyuncuları getir
        Task<List<Player>> GetPlayersByPositionAsync(string position); // Mevkisine göre
        Task<Player?> GetPlayerByIdAsync(int id);                // ID ile getir
        Task<Player?> GetPlayerBySlugAsync(string slug);         // URL'den getir
        Task AddPlayerAsync(Player player);                      // Ekle
    }
}
