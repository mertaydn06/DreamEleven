using DreamEleven.Business.Abstract;
using DreamEleven.DataAccess.Abstract;
using DreamEleven.Entities;

namespace DreamEleven.Business.Concrete
{
    public class PlayerManager : IPlayerService  // IPlayerService'i uygulayan sınıf — Controller buradan çağırır
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerManager(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }


        public Task<List<Player>> GetAllPlayersAsync()
        {
            return _playerRepository.GetAllPlayersAsync();
        }

        public Task<Player?> GetPlayerByIdAsync(int id)
        {
            return _playerRepository.GetPlayerByIdAsync(id);
        }

        public async Task<Player?> GetBySlugAsync(string slug)
        {
            return await _playerRepository.GetBySlugAsync(slug);
        }

        public Task AddPlayerAsync(Player player)
        {
            return _playerRepository.AddPlayerAsync(player);
        }

        public async Task<List<Player>> GetPlayersByNameAsync(string query)
        {
            return await _playerRepository.GetPlayersByNameAsync(query);
        }

    }
}
