using DreamEleven.Entities;

namespace DreamEleven.Business.Abstract
{
    // Servis katmanında controller'ın çağıracağı işlemleri tanımlar
    public interface ITeamService
    {
        Task<List<Team>> GetAllTeamsAsync();             // Tüm takımları getir
        Task<Team?> GetTeamByIdAsync(int id);            // Belirli ID'ye göre takım getir
        Task<List<Team>> GetTeamsByUserIdAsync(string userId); // Kullanıcıya ait takımları getir
        Task CreateTeamAsync(Team team);                 // Yeni takım ekle
        Task DeleteTeamAsync(int id);                    // Takımı sil
        Task UpdateTeamAsync(Team team);                 // Takımı güncelle
    }
}
