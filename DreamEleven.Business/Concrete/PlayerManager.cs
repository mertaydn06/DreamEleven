using DreamEleven.Business.Abstract;
using DreamEleven.DataAccess.Abstract;
using DreamEleven.Entities;

namespace DreamEleven.Business.Concrete
{
    public class PlayerManager : IPlayerService
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

        public Task<List<Player>> GetPlayersByPositionAsync(string position)
        {
            return _playerRepository.GetPlayersByPositionAsync(position);
        }

        public Task<Player?> GetPlayerBySlugAsync(string slug)
        {
            return _playerRepository.GetPlayerBySlugAsync(slug);
        }

        public Task AddPlayerAsync(Player player)
        {
            return _playerRepository.AddPlayerAsync(player);
        }
    }
}
