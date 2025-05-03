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
            _playerRepository = playerRepository;  // Dependency Injection (DI) kullanılarak repository sınıfı alınıyor
        }

        // Tüm oyuncuları asenkron olarak getirir
        public Task<List<Player>> GetAllPlayersAsync()
        {
            return _playerRepository.GetAllPlayersAsync();
        }

        // ID'ye göre oyuncu getirir
        public Task<Player?> GetPlayerByIdAsync(int id)
        {
            return _playerRepository.GetPlayerByIdAsync(id);
        }

        // Slug'a göre oyuncunun takımlarını ve o takımların oyuncularını getirir
        public async Task<Player?> GetBySlugAsync(string slug)
        {
            return await _playerRepository.GetBySlugAsync(slug);
        }

        // Arama sorgusuna göre oyuncuları getirir
        public async Task<List<Player>> GetPlayersByNameAsync(string query)
        {
            return await _playerRepository.GetPlayersByNameAsync(query);
        }
    }
}
