using DreamEleven.Business.Abstract;
using DreamEleven.Entities;
using Microsoft.AspNetCore.Mvc;
using DreamEleven.Web.Models;
using Microsoft.AspNetCore.Identity;
using DreamEleven.Identity;
using DreamEleven.Web.Helpers;

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


        public async Task<IActionResult> Create(string? formation)
        {
            var selectedFormation = formation ?? "4-4-2"; // eğer boş gelirse default ver

            var model = new CreateTeamViewModel
            {
                Formation = selectedFormation,
                Players = FormationHelper.GetSlots(selectedFormation)
                    .Select(slot => new TeamPlayerInput { PositionSlot = slot })
                    .ToList()
            };

            ViewBag.Formations = new List<string> { "4-4-2", "4-3-3", "3-5-2", "5-3-2", "4-5-1", "3-4-3" };
            ViewBag.AllPlayers = await _playerService.GetAllPlayersAsync(); // null değilse

            return View(model);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeamViewModel model)
        {
            if (!ModelState.IsValid || model.Players.Count != 11)
            {
                ModelState.AddModelError("", "Tüm pozisyonlara oyuncu seçmelisiniz.");
                ViewBag.Formations = new List<string> { "4-4-2", "4-3-3", "3-5-2", "5-3-2", "4-5-1", "3-4-3" };
                return View(model);
            }

            var userId = "d7a27839-7375-46bd-81ad-6a5db45db25a";

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

            // ✔️ Servis üzerinden kayıt
            await _teamService.CreateTeamAsync(team);

            return RedirectToAction("Details", new { id = team.Id });
        }

    }
}