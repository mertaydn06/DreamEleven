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

    public IActionResult Index()
    {
        return View();
    }


    [Route("player/{slug}")]
    public async Task<IActionResult> PlayerDetails(string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return NotFound();

        var player = await _playerService.GetBySlugAsync(slug);

        if (player == null)
            return NotFound();

        var teamPlayers = player.TeamPlayers.ToList();

        var teams = teamPlayers
            .Select(tp => tp.Team)
            .DistinctBy(t => t.Id)
            .ToList();

        var teamOwners = new Dictionary<int, string>();
        var teamOwnerImages = new Dictionary<int, string>();

        foreach (var team in teams)
        {
            var user = await _userManager.FindByIdAsync(team.UserId);

            if (user != null)
            {
                teamOwners[team.Id] = user.UserName!;
                teamOwnerImages[team.Id] = user.Image ?? "/images/User.jpg"; // Eğer user'ın resmi yoksa default
            }
            else
            {
                teamOwners[team.Id] = "Bilinmeyen";
                teamOwnerImages[team.Id] = "/images/User.jpg";
            }
        }

        ViewBag.TeamOwners = teamOwners;
        ViewBag.TeamOwnerImages = teamOwnerImages;

        return View(teamPlayers);
    }






}