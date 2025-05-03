using DreamEleven.Business.Abstract;
using DreamEleven.DataAccess.Abstract;
using DreamEleven.Entities;

namespace DreamEleven.Business.Concrete
{
    public class TeamManager : ITeamService  // ITeamService'i uygulayan sınıf — Controller buradan çağırır
    {
        private readonly ITeamRepository _teamRepository;

        public TeamManager(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;  // Dependency Injection (DI) kullanılarak repository sınıfı enjekte ediliyor
        }

        // Tüm takımları asenkron olarak getirir
        public Task<List<Team>> GetAllTeamsAsync()
        {
            return _teamRepository.GetAllTeamsAsync();
        }

        // ID'ye göre takımı getirir
        public Task<Team?> GetTeamByIdAsync(int id)
        {
            return _teamRepository.GetTeamByIdAsync(id);
        }

        // Kullanıcıya ait tüm takımları getirir
        public Task<List<Team>> GetTeamsByUserIdAsync(string userId)
        {
            return _teamRepository.GetTeamsByUserIdAsync(userId);
        }

        // Belirli bir oyuncuya ait takımları getirir
        public async Task<List<Team>> GetTeamsByPlayerIdAsync(int playerId)
        {
            return await _teamRepository.GetTeamsByPlayerIdAsync(playerId);
        }


        // Yeni takım ekler
        public Task CreateTeamAsync(Team team)
        {
            return _teamRepository.CreateTeamAsync(team);
        }

        // Takımı siler
        public Task DeleteTeamAsync(int id)
        {
            return _teamRepository.DeleteTeamAsync(id);
        }

        // Takımı günceller
        public Task UpdateTeamAsync(Team team)
        {
            return _teamRepository.UpdateTeamAsync(team);
        }
    }
}
