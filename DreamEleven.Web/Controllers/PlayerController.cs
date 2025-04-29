using Microsoft.AspNetCore.Mvc;
using DreamEleven.Business.Abstract;
using Microsoft.AspNetCore.Identity;
using DreamEleven.Identity;

namespace DreamEleven.Web.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly UserManager<User> _userManager;

        public PlayerController(IPlayerService playerService, UserManager<User> userManager)
        {
            _playerService = playerService;
            _userManager = userManager;
        }


        [Route("player/{slug}")]
        public async Task<IActionResult> PlayerDetails(string slug, int page = 1)
        {
            if (string.IsNullOrEmpty(slug))
                return NotFound();

            var player = await _playerService.GetBySlugAsync(slug);
            if (player == null)
                return NotFound();

            var sessionKey = $"RandomTeamOrder_{slug}";

            var teamPlayers = player.TeamPlayers?.ToList() ?? new List<DreamEleven.Entities.TeamPlayer>();

            var allTeams = teamPlayers
                .Select(tp => tp.Team)
                .DistinctBy(t => t.Id)
                .ToList();

            // Sıralı ID listesi Session'da var mı kontrol ediyoruz
            var sessionData = HttpContext.Session.GetString(sessionKey);
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

            // Şu anda elimizde rastgele sıralanmış ID'ler var, bu ID'lere göre sıralıyoruz
            allTeams = shuffledIds
                .Select(id => allTeams.First(t => t.Id == id))
                .ToList();

            var pageSize = 3;
            var pagedTeams = allTeams
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedTeamPlayers = teamPlayers
                .Where(tp => pagedTeams.Any(t => t.Id == tp.TeamId))
                .ToList();

            var teamOwners = new Dictionary<int, string>();
            var teamOwnerImages = new Dictionary<int, string>();

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

            ViewBag.AllTeamsForSidebar = allTeams;
            ViewBag.TeamOwners = teamOwners;
            ViewBag.TeamOwnerImages = teamOwnerImages;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)allTeams.Count / pageSize);
            ViewBag.Player = player;

            return View(pagedTeamPlayers);
        }


        [HttpGet]
        [Route("Player/SearchPlayers")]
        public async Task<IActionResult> SearchPlayers(string query)
        {
            if (string.IsNullOrEmpty(query))
                return Json(new List<object>());

            var players = await _playerService.GetPlayersByNameAsync(query);

            var result = players.Select(p => new
            {
                name = p.Name,
                slug = p.Slug,
                imageUrl = p.ImageUrl
            });

            return Json(result);
        }
    }
}
