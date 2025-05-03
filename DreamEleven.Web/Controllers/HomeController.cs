using Microsoft.AspNetCore.Identity;
using DreamEleven.Identity;
using Microsoft.AspNetCore.Mvc;
using DreamEleven.Business.Abstract;
using DreamEleven.Entities;

namespace DreamEleven.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;    // Kullanıcı yönetimi için UserManager servisi
        private readonly ITeamService _teamService;         // Takım işlemleri için ITeamService
        private readonly IPlayerService _playerService;     // Oyuncu işlemleri için IPlayerService

        public HomeController(UserManager<User> userManager, ITeamService teamService, IPlayerService playerService)
        {
            _userManager = userManager;        // UserManager servisi sınıfa atanır
            _teamService = teamService;        // Takım servisi sınıfa atanır
            _playerService = playerService;    // Oyuncu servisi sınıfa atanır
        }


        public async Task<IActionResult> Index(int page = 1)
        {
            var teams = await _teamService.GetAllTeamsAsync();  // Tüm takımlar veritabanından alınır.
            var players = await _playerService.GetAllPlayersAsync();  // Tüm oyuncular veritabanından alınır.

            var pageSize = 3;  // Her sayfada gösterilecek takım sayısı 3 olarak ayarlanır.

            var pagedTeams = teams
                .OrderByDescending(t => t.CreatedAt)  // Takımlar oluşturulma tarihine göre sıralanır.
                .Skip((page - 1) * pageSize)          // Sayfalama işlemi, geçerli sayfaya göre takım verileri atlanır.
                .Take(pageSize)                       // Sayfa başına 3 takım alınır.
                .ToList();



            // Popüler Oyuncular
            List<Player> popularPlayers;

            if (HttpContext.Session.GetString("PopularPlayers") == null)  // Eğer oturumda popüler oyuncular verisi yoksa
            {
                var selectedPlayers = players
                    .OrderBy(x => Guid.NewGuid())  // Oyuncular rastgele sıralanır.
                    .Take(8)                       // 8 oyuncu seçilir.
                    .Select(p => p.Name)           // Yalnızca oyuncuların isimleri seçilir.
                    .ToList();


                // Seçilen oyuncular oturumda saklanır
                HttpContext.Session.SetString("PopularPlayers", System.Text.Json.JsonSerializer.Serialize(selectedPlayers));
                popularPlayers = players.Where(p => selectedPlayers.Contains(p.Name)).ToList();  // Seçilen oyuncular veritabanından çekilir.
            }
            else
            {
                var selectedPlayers = System.Text.Json.JsonSerializer.Deserialize<List<string>>(HttpContext.Session.GetString("PopularPlayers")!);
                popularPlayers = players.Where(p => selectedPlayers!.Contains(p.Name)).ToList();  // Oturumda saklanan popüler oyuncular kullanılır.
            }


            // Haftanın Takımları
            List<Team> lastTeams;

            if (HttpContext.Session.GetString("LastTeams") == null)
            {
                var selectedTeamIds = teams
                    .OrderBy(x => Guid.NewGuid())  // Takımlar rastgele sıralanır.
                    .Take(10)                      // İlk 10 takım seçilir.
                    .Select(t => t.Id)             // Yalnızca takım ID'leri seçilir.
                    .ToList();


                // Seçilen takımlar oturumda saklanır.
                HttpContext.Session.SetString("LastTeams", System.Text.Json.JsonSerializer.Serialize(selectedTeamIds));
                lastTeams = selectedTeamIds
                    .Select(id => teams.First(t => t.Id == id))  // ID'lere göre takımlar seçilir.
                    .ToList();
            }
            else
            {
                var selectedTeamIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(HttpContext.Session.GetString("LastTeams")!);
                lastTeams = teams.Where(t => selectedTeamIds!.Contains(t.Id)).ToList();  // Oturumda saklanan takımlar kullanılır.
            }


            // Haftanın Oyuncusu
            Player? randomPlayer;
            if (HttpContext.Session.GetString("RandomPlayer") == null)
            {
                var selected = players.OrderBy(x => Guid.NewGuid()).FirstOrDefault();  // Rastgele bir oyuncu seçilir.

                if (selected != null)
                {
                    HttpContext.Session.SetString("RandomPlayer", selected.Name);  // Seçilen oyuncunun ismi oturumda saklanır.
                }
                randomPlayer = selected;  // Rastgele seçilen oyuncu atanır.
            }
            else
            {
                var selectedName = HttpContext.Session.GetString("RandomPlayer");

                randomPlayer = players.FirstOrDefault(p => p.Name == selectedName);
            }


            var teamOwners = new Dictionary<int, string>();  // Takım sahiplerini tutar.
            var teamOwnerImages = new Dictionary<int, string>();  // Takım sahiplerinin resimlerini tutar.

            foreach (var team in pagedTeams)
            {
                var user = await _userManager.FindByIdAsync(team.UserId);  // Takım sahibinin kullanıcı bilgisi alınır.

                if (user != null)
                {
                    teamOwners[team.Id] = user.UserName!;  // Takım sahibinin kullanıcı adı dictionary'e eklenir.
                    teamOwnerImages[team.Id] = user.Image ?? "/images/User.jpg";
                }
                else
                {
                    teamOwners[team.Id] = "Bilinmeyen";
                    teamOwnerImages[team.Id] = "/images/User.jpg";
                }
            }


            // Veritabanından alınan ve oturumdan getirilen veriler ViewBag ile view'a aktarılır.
            ViewBag.Teams = pagedTeams;
            ViewBag.PopularPlayers = popularPlayers;
            ViewBag.LastTeams = lastTeams;

            ViewBag.TeamOwners = teamOwners;
            ViewBag.TeamOwnerImages = teamOwnerImages;

            ViewBag.RandomPlayer = randomPlayer;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)teams.Count / pageSize);

            return View(pagedTeams);
        }
    }
}
