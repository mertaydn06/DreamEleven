using Microsoft.AspNetCore.Mvc;
using DreamEleven.Business.Abstract;
using Microsoft.AspNetCore.Identity;
using DreamEleven.Identity;

namespace DreamEleven.Web.Controllers
{
    public class PlayerController : Controller
    {
        private readonly UserManager<User> _userManager;    // Kullanıcı yönetimi için UserManager servisi
        private readonly IPlayerService _playerService;     // Oyuncu işlemleri için IPlayerService

        public PlayerController(UserManager<User> userManager, IPlayerService playerService)
        {
            _userManager = userManager;        // UserManager servisi sınıfa atanır
            _playerService = playerService;    // Oyuncu servisi sınıfa atanır
        }


        [Route("player/{slug}")]  // URL'nin slug kısmına göre oyuncu detaylarını gösterir.
        public async Task<IActionResult> PlayerDetails(string slug, int page = 1)
        {
            if (string.IsNullOrEmpty(slug))
                return NotFound();

            var player = await _playerService.GetBySlugAsync(slug);  // Sluga göre oyuncu verisi alınır.

            if (player == null)
                return NotFound();


            var sessionKey = $"RandomTeamOrder_{slug}";  // Her oyuncuya özel session anahtarı oluşturulur.

            var teamPlayers = player.TeamPlayers?.ToList() ?? new List<DreamEleven.Entities.TeamPlayer>();  // Oyuncunun takımlarına ait oyuncular alınır.

            var allTeams = teamPlayers
                .Select(tp => tp.Team)   // Takımların listesi alınır.
                .DistinctBy(t => t.Id)   // Aynı takımların tekrarını engeller.
                .ToList();


            var sessionData = HttpContext.Session.GetString(sessionKey);  // Session'da veriler var mı kontrol edilir.

            List<int> shuffledIds;

            if (string.IsNullOrEmpty(sessionData))
            {
                // Session'da yoksa ID'leri rastgele sırala ve Session'a kaydediyoruz
                shuffledIds = allTeams.Select(t => t.Id).OrderBy(_ => Guid.NewGuid()).ToList();
                var serialized = System.Text.Json.JsonSerializer.Serialize(shuffledIds);
                HttpContext.Session.SetString(sessionKey, serialized);
            }
            else
            {
                // Session'dan sırayı alıyoruz
                shuffledIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(sessionData)!;
            }

            allTeams = shuffledIds
                .Select(id => allTeams.First(t => t.Id == id))  // ID'lere göre takımlar sıralanır.
                .ToList();



            var pageSize = 3;
            var pagedTeams = allTeams
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedTeamPlayers = teamPlayers
                .Where(tp => pagedTeams.Any(t => t.Id == tp.TeamId))
                .ToList();



            var teamOwners = new Dictionary<int, string>();       // Takım sahiplerinin bilgilerini tutar.
            var teamOwnerImages = new Dictionary<int, string>();  // Takım sahiplerinin resimlerini tutar.

            foreach (var team in allTeams)
            {
                var user = await _userManager.FindByIdAsync(team.UserId);

                if (user != null)
                {
                    teamOwners[team.Id] = user.UserName!;
                    teamOwnerImages[team.Id] = user.Image ?? "/images/User.jpg";
                }
                else
                {
                    teamOwners[team.Id] = "Bilinmeyen";
                    teamOwnerImages[team.Id] = "/images/User.jpg";
                }
            }


            // Veritabanından alınan ve oturumdan getirilen veriler ViewBag ile view'a aktarılır.
            ViewBag.Player = player;
            ViewBag.AllTeamsForSidebar = allTeams;

            ViewBag.TeamOwners = teamOwners;
            ViewBag.TeamOwnerImages = teamOwnerImages;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)allTeams.Count / pageSize);

            return View(pagedTeamPlayers);
        }


        [HttpGet]
        [Route("Player/SearchPlayers")]  // Oyuncu arama için bir route tanımlanır.
        public async Task<IActionResult> SearchPlayers(string query)
        {
            if (string.IsNullOrEmpty(query))
                return Json(new List<object>());

            var players = await _playerService.GetPlayersByNameAsync(query);  // Arama metnine göre oyuncular getirilir.

            // Arama sonuçlarını JSON formatında döndürür.
            var result = players.Select(p => new
            {
                name = p.Name,
                slug = p.Slug,
                imageUrl = p.ImageUrl
            });

            return Json(result);  // JSON formatında sonuç döndürülür.
        }
    }
}
