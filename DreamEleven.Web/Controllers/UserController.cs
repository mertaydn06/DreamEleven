using DreamEleven.Business.Abstract;
using DreamEleven.Entities;
using DreamEleven.Identity;
using DreamEleven.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DreamEleven.Web.Controllers
{
    [Route("profile")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITeamService _teamService;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, ITeamService teamService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _teamService = teamService;           // Takım işlemleri servisi
        }


        [HttpGet("{username}")]
        public async Task<IActionResult> Profile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);  // Gelen username' ait kullanıcı bilgileri user'a aktarılır.

            if (user == null)
                return NotFound();

            var teams = await _teamService.GetTeamsByUserIdAsync(user.Id);  // Kullanıcıya ait takımları getirir.

            var model = new UserProfileViewModel
            {
                User = user,
                Teams = teams,
                IsCurrentUser = User.Identity!.Name == username
            };

            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new EditUserViewModel
            {
                Username = user.UserName!,
                Email = user.Email!,
                Image = user.Image
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.UserName = model.Username;
            user.Image = model.Image;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!passResult.Succeeded)
                {
                    foreach (var err in passResult.Errors)
                        ModelState.AddModelError("", err.Description);
                    return View(model);
                }
            }

            if (updateResult.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Profile", new { username = user.UserName });
            }

            foreach (var err in updateResult.Errors)
                ModelState.AddModelError("", err.Description);

            return View(model);
        }

    }
}
