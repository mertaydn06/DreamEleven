using DreamEleven.Business.Abstract;
using DreamEleven.Entities;
using Microsoft.AspNetCore.Mvc;
using DreamEleven.Web.Models;
using Microsoft.AspNetCore.Identity;
using DreamEleven.Identity;
using DreamEleven.Web.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace DreamEleven.Web.Controllers
{
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;
        private readonly UserManager<User> _userManager;

        public TeamController(ITeamService teamService, IPlayerService playerService, UserManager<User> userManager)
        {
            _teamService = teamService;           // Takım işlemleri servisi
            _playerService = playerService;       // Oyuncu işlemleri servisi (listeleme vs.)
            _userManager = userManager;
        }


        [Authorize]
        public async Task<IActionResult> Create(string formation = "4-4-2")
        {
            // Eğer URL'den formasyon parametresi geldiyse onu kullan, yoksa varsayılan "4-4-2" kullan
            var defaultFormation = formation ?? "4-4-2";

            var model = new CreateTeamViewModel
            {
                Formation = defaultFormation,
                Players = FormationHelper.GetSlots(defaultFormation)
                    .Select(slot => new TeamPlayerInput { PositionSlot = slot })
                    .ToList()
            };

            ViewBag.Formations = FormationHelper.AvailableFormations;
            ViewBag.AllPlayers = await _playerService.GetAllPlayersAsync();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTeamViewModel model)
        {
            if (!ModelState.IsValid || model.Players.Any(p => p.PlayerId <= 0))
            {
                ModelState.AddModelError("", "Tüm pozisyonlara oyuncu seçmelisiniz.");
                ViewBag.Formations = FormationHelper.AvailableFormations;
                ViewBag.AllPlayers = await _playerService.GetAllPlayersAsync();
                return View(model);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var team = new Team
            {
                TeamName = model.TeamName,
                Formation = model.Formation,
                CreatedAt = DateTime.UtcNow,
                UserId = userId!,
                TeamPlayers = model.Players.Select(p => new TeamPlayer
                {
                    PlayerId = p.PlayerId,
                    PositionSlot = p.PositionSlot
                }).ToList()
            };

            await _teamService.CreateTeamAsync(team);

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Details(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null) return NotFound();

            var commentVMs = new List<CommentViewModel>();

            foreach (var comment in team.Comments.OrderByDescending(c => c.CreatedAt))
            {
                var user = await _userManager.FindByIdAsync(comment.UserId);

                commentVMs.Add(new CommentViewModel
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    UserName = user?.UserName ?? "Bilinmeyen",
                    UserImage = string.IsNullOrEmpty(user?.Image) ? "/images/User.jpg"
                    : user.Image
                });
            }

            var owner = await _userManager.FindByIdAsync(team.UserId);
            ViewBag.TeamOwner = owner;


            ViewBag.Comments = commentVMs;
            return View(team);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteTeam(int teamId)
        {
            var team = await _teamService.GetTeamByIdAsync(teamId);

            if (team == null)
                return NotFound();

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (team.UserId != currentUserId)
                return Forbid();

            await _teamService.DeleteTeamAsync(teamId);

            return Redirect($"/profile/{User.Identity!.Name}");
        }

    }
}
