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
            var defaultFormation = string.IsNullOrEmpty(formation) ? "4-4-2" : formation;

            var model = new CreateTeamViewModel
            {
                Formation = defaultFormation,
                Players = FormationHelper.GetSlots(defaultFormation)
                    .Select(slot => new TeamPlayerInput { PositionSlot = slot })
                    .ToList()
            };

            ViewBag.Formations = new List<string> { "4-4-2", "4-3-3", "3-5-2", "5-3-2", "4-5-1", "3-4-3" };
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
                ViewBag.Formations = new List<string> { "4-4-2", "4-3-3", "3-5-2", "5-3-2", "4-5-1", "3-4-3" };
                ViewBag.AllPlayers = await _playerService.GetAllPlayersAsync();
                return View(model);
            }

            var userId = "01a8bbc4-ddf6-4ee7-9dfa-311d82b64e3f"; // oturum açmış kullanıcıdan id alıyoruz

            var team = new Team
            {
                TeamName = model.TeamName,
                Formation = model.Formation,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
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

            var commentVMs = new List<CommentViewModel>();  // 

            foreach (var comment in team.Comments.OrderByDescending(c => c.CreatedAt))
            {
                var user = await _userManager.FindByIdAsync(comment.UserId);
                commentVMs.Add(new CommentViewModel
                {
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    UserName = user?.UserName ?? "Bilinmeyen"
                });
            }

            ViewBag.Comments = commentVMs;  // Yorumları ViewBag ile View'a gönderdik.ç
            return View(team);
        }

    }
}
