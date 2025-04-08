using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DreamEleven.Web.Models;
using DreamEleven.Business.Abstract;

namespace DreamEleven.Web.Controllers;

public class HomeController : Controller
{
    private readonly ITeamService _teamService;
    private readonly IPlayerService _playerService;

    public HomeController(ITeamService teamService, IPlayerService playerService)
    {
        _teamService = teamService;
        _playerService = playerService;
    }


    public IActionResult Index()
    {
        return View();
    }


    public async Task<IActionResult> PlayerDetails(string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return NotFound();

        var player = await _playerService.GetBySlugAsync(slug);

        if (player == null)
            return NotFound();


        var teamPlayers = player.TeamPlayers.ToList();  // TeamPlayer'ları Include ederek aldık.

        return View(teamPlayers); // 👈 direkt View'a gönderiyoruz
    }




}