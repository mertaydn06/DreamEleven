using DreamEleven.Entities;

namespace DreamEleven.Business.Abstract
{
    // Servis katmanında controller'ın çağıracağı işlemleri tanımlar
    public interface IPlayerService
    {
        Task<List<Player>> GetAllPlayersAsync();                   // Tüm futbolcuları getirir
        Task<Player?> GetPlayerByIdAsync(int id);                  // Belirli ID'ye göre futbolcu getirir
        Task<Player?> GetBySlugAsync(string slug);                 // Futbolcunun takımlarını ve takımların oyuncularını getirir
        Task<List<Player>> GetPlayersByNameAsync(string query);    // Futbolcu arama kelimesine göre oyuncuyu getirir
    }
}
