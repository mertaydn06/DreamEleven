using Microsoft.AspNetCore.Mvc;
using DreamEleven.Identity;
using Microsoft.AspNetCore.Identity;

namespace DreamEleven.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;      // Kullanıcı yönetimi için UserManager servisi
        private readonly SignInManager<User> _signInManager;  // Kullanıcı oturum açma işlemleri için SignInManager servisi

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;       // UserManager servisi sınıfa atanır
            _signInManager = signInManager;   // SignInManager servisi sınıfa atanır
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)  // Formdan gelen yeni kayıt VM ile burada karşılanır.
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User  // Yeni bir User nesnesi oluşturduk.
            {
                UserName = model.UserName,        // Formdan gelen kullanıcı adı
                Email = model.Email,              // Formdan gelen e-posta adresi
                CreatedAt = DateTime.UtcNow,
                Image = "/images/users/User.jpg"  // Varsayılan profil resmi
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);  // Şifreyi hashleyerek (PasswordHasher) veritabanına kaydettik.

            if (result.Succeeded)  // Eğer kullanıcı başarıyla oluşturulmuşsa
            {
                await _userManager.AddToRoleAsync(user, "User");  // Kullanıcıya 'User' rolü atanır.
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }


        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;  // returnUrl'i ViewBag ile view'a gönderir (giriş başarılı olduktan sonra bu URL'ye yönlendirme için).

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

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);  // Şifreyi kontrol ederek giriş işlemini yapar.

            if (result.Succeeded)
            {
                return Redirect(returnUrl ?? "/");  // Eğer returnUrl varsa, oraya yönlendirir; yoksa ana sayfaya gider.
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
