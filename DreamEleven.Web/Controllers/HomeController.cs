using Microsoft.AspNetCore.Identity;
using DreamEleven.Identity;
using Microsoft.AspNetCore.Mvc;
using DreamEleven.Business.Abstract;
using DreamEleven.Entities;

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

        // 🔥 Popüler Oyuncular
        List<Player> popularPlayers;
        if (HttpContext.Session.GetString("PopularPlayers") == null)
        {
            var selectedPlayers = players
                .OrderBy(x => Guid.NewGuid())
                .Take(8)
                .Select(p => p.Name)  // Sadece isimleri kaydedelim
                .ToList();

            HttpContext.Session.SetString("PopularPlayers", System.Text.Json.JsonSerializer.Serialize(selectedPlayers));
            popularPlayers = players.Where(p => selectedPlayers.Contains(p.Name)).ToList();
        }
        else
        {
            var selectedPlayers = System.Text.Json.JsonSerializer.Deserialize<List<string>>(HttpContext.Session.GetString("PopularPlayers")!);
            popularPlayers = players.Where(p => selectedPlayers!.Contains(p.Name)).ToList();
        }

        // 🔥 Haftanın Takımları
        List<Team> lastTeams;
        if (HttpContext.Session.GetString("LastTeams") == null)
        {
            var selectedTeamIds = teams
                .OrderBy(x => Guid.NewGuid())
                .Take(10)
                .Select(t => t.Id)
                .ToList();

            HttpContext.Session.SetString("LastTeams", System.Text.Json.JsonSerializer.Serialize(selectedTeamIds));
            lastTeams = selectedTeamIds
                .Select(id => teams.First(t => t.Id == id))
                .ToList();
        }
        else
        {
            var selectedTeamIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(HttpContext.Session.GetString("LastTeams")!);
            lastTeams = teams.Where(t => selectedTeamIds!.Contains(t.Id)).ToList();
        }

        // 🔥 Haftanın Oyuncusu
        Player? randomPlayer;
        if (HttpContext.Session.GetString("RandomPlayer") == null)
        {
            var selected = players.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            if (selected != null)
            {
                HttpContext.Session.SetString("RandomPlayer", selected.Name);
            }
            randomPlayer = selected;
        }
        else
        {
            var selectedName = HttpContext.Session.GetString("RandomPlayer");
            randomPlayer = players.FirstOrDefault(p => p.Name == selectedName);
        }

        //  Burada takımları oluşturan kullanıcı bilgilerini topluyoruz
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
}
