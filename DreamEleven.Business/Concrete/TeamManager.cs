using DreamEleven.Business.Abstract;
using DreamEleven.DataAccess.Abstract;
using DreamEleven.Entities;

namespace DreamEleven.Business.Concrete
{
    // ITeamService'i uygulayan sınıf — Controller buradan çağırır
    public class TeamManager : ITeamService
    {
        private readonly ITeamRepository _teamRepository;  // Repository katmanına bağımlılık

        public TeamManager(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public Task<List<Team>> GetAllTeamsAsync()
        {
            return _teamRepository.GetAllTeamsAsync();     // Veri erişimi repository'den alınır
        }

        public Task<Team?> GetTeamByIdAsync(int id)
        {
            return _teamRepository.GetTeamByIdAsync(id);
        }

        public Task<List<Team>> GetTeamsByUserIdAsync(string userId)
        {
            return _teamRepository.GetTeamsByUserIdAsync(userId);
        }

        public Task CreateTeamAsync(Team team)
        {
            return _teamRepository.CreateTeamAsync(team);
        }

        public Task DeleteTeamAsync(int id)
        {
            return _teamRepository.DeleteTeamAsync(id);
        }

        public Task UpdateTeamAsync(Team team)
        {
            return _teamRepository.UpdateTeamAsync(team);
        }
    }
}
