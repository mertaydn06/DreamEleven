using DreamEleven.Entities;

namespace DreamEleven.Business.Abstract
{
    // Servis katmanında controller'ın çağıracağı işlemleri tanımlar
    public interface ITeamService
    {
        Task<List<Team>> GetAllTeamsAsync();                       // Tüm takımları getirir
        Task<Team?> GetTeamByIdAsync(int id);                      // Belirli ID'ye göre takım getirir
        Task<List<Team>> GetTeamsByUserIdAsync(string userId);     // Kullanıcıya ait takımları getirir
        Task<List<Team>> GetTeamsByPlayerIdAsync(int playerId);    // Belirli futbolcu ID'sine göre takım getirir

        Task CreateTeamAsync(Team team);                           // Yeni takım ekler
        Task DeleteTeamAsync(int id);                              // Takımı siler
        Task UpdateTeamAsync(Team team);                           // Takımı günceller
    }
}
