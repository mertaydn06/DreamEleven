using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DreamEleven.Web.Models;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DreamEleven.Identity;
using Microsoft.AspNetCore.Identity;

namespace DreamEleven.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }


    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)  // Formdan gelen yeni kayıt VM burada karşılanır.
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            CreatedAt = DateTime.UtcNow
        };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);  // Şifreyi hashleyerek (PasswordHasher) veritabanına kaydeder.

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }








}
