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
            _teamRepository = teamRepository;
        }


        public Task<List<Team>> GetAllTeamsAsync()
        {
            return _teamRepository.GetAllTeamsAsync();
        }

        public Task<Team?> GetTeamByIdAsync(int id)
        {
            return _teamRepository.GetTeamByIdAsync(id);
        }

        public Task<List<Team>> GetTeamsByUserIdAsync(string userId)
        {
            return _teamRepository.GetTeamsByUserIdAsync(userId);
        }

        public async Task<List<Team>> GetTeamsByPlayerIdAsync(int playerId)
        {
            return await _teamRepository.GetTeamsByPlayerIdAsync(playerId);
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

        public async Task AddCommentAsync(Comment comment)
        {
            await _teamRepository.AddCommentAsync(comment);
        }

    }
}
