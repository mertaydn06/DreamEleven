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

            return View(pagedTeamPlayers);
        }


        [HttpGet]
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
