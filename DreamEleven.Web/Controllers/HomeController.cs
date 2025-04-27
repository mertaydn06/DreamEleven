using Microsoft.AspNetCore.Identity;
using DreamEleven.Identity;
using Microsoft.AspNetCore.Mvc;
using DreamEleven.Business.Abstract;

public class HomeController : Controller
{
    private readonly ITeamService _teamService;
    private readonly IPlayerService _playerService;
    private readonly UserManager<User> _userManager;

    public HomeController(ITeamService teamService, IPlayerService playerService, UserManager<User> userManager)
    {
        _teamService = teamService;
        _playerService = playerService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var teams = await _teamService.GetAllTeamsAsync();
        var players = await _playerService.GetAllPlayersAsync();

        var pageSize = 3; // Her sayfada 3 takım

        var pagedTeams = teams
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var popularPlayers = players
            .OrderBy(x => Guid.NewGuid()) // Rastgele popüler oyuncular
            .Take(8)
            .ToList();

        var lastTeams = teams
            .OrderBy(x => Guid.NewGuid()) // Rastgele takımlar (Haftanın takımı)
            .Take(10)
            .ToList();

        var randomPlayer = players  // Rastgele oyuncu
            .OrderBy(x => Guid.NewGuid())
            .FirstOrDefault();

        // ✨ Burada takımları oluşturan kullanıcı bilgilerini topluyoruz
        var teamOwners = new Dictionary<int, string>();
        var teamOwnerImages = new Dictionary<int, string>();

        foreach (var team in pagedTeams)
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

        // ✨ ViewBag'lere atıyoruz
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



    [Route("player/{slug}")]
    public async Task<IActionResult> PlayerDetails(string slug, int page = 1)
    {
        if (string.IsNullOrEmpty(slug))
            return NotFound();

        var player = await _playerService.GetBySlugAsync(slug);
        if (player == null)
            return NotFound();

        var teamPlayers = player.TeamPlayers.ToList();

        var allTeams = teamPlayers
            .Select(tp => tp.Team)
            .DistinctBy(t => t.Id)
            .OrderByDescending(t => t.CreatedAt)
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

        foreach (var team in allTeams)  // <-- burada tüm takımların sahiplerini getiriyoruz
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

        // 👇 Bu satırı yeni ekliyoruz: Sol liste için tüm takımları ViewBag'e koy.
        ViewBag.AllTeamsForSidebar = allTeams;

        ViewBag.TeamOwners = teamOwners;
        ViewBag.TeamOwnerImages = teamOwnerImages;

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)allTeams.Count / pageSize);

        return View(pagedTeamPlayers);
    }
}
