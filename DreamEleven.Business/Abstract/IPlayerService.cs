using DreamEleven.Entities;

namespace DreamEleven.Business.Abstract
{
    public interface IPlayerService
    {
        Task<List<Player>> GetAllPlayersAsync();
        Task<List<Player>> GetPlayersByPositionAsync(string position);
        Task<Player?> GetPlayerBySlugAsync(string slug);
        Task AddPlayerAsync(Player player);
    }
}
