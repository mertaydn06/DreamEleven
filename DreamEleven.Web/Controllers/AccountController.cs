using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DreamEleven.Web.Models;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DreamEleven.Identity;
using Microsoft.AspNetCore.Identity;

namespace DreamEleven.Web.Controllers
{

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
                CreatedAt = DateTime.UtcNow,
                Image = "/images/users/User.jpg"
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


        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);  // Mail'e ait kullanıcıyı user'a atar.

            if (user == null)
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                return Redirect(returnUrl ?? "/");  // Giriş başarılı ise Login'den gelinen sayfaya gider yoksa anasayfaya gider.
            }

            ModelState.AddModelError("", "Giriş başarısız.");
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();  // Kullanıcının oturumunu kapatır. Cookie'yi siler.

            return RedirectToAction("Index", "Home");
        }
    }
}