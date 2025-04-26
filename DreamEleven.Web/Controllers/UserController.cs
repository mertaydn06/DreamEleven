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
        private readonly ICommentService _commentService;


        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, ITeamService teamService, ICommentService commentService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _teamService = teamService;           // Takım işlemleri servisi
            _commentService = commentService;
        }


        [HttpGet("{username}")]
        public async Task<IActionResult> Profile(string username, int page = 1)
        {
            var user = await _userManager.FindByNameAsync(username);  // Gelen username' ait kullanıcı bilgileri user'a aktarılır.

            if (user == null)
                return NotFound();

            var teams = await _teamService.GetTeamsByUserIdAsync(user.Id);  // Kullanıcıya ait takımları getirir.

            // Sayfalama işlemi 👇
            int pageSize = 3;  // Bir sayfada gösterilecek takım sayısı

            var pagedTeams = teams
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new UserProfileViewModel
            {
                User = user,
                Teams = pagedTeams,   // Sadece o sayfadaki takımlar
                IsCurrentUser = User.Identity!.Name == username
            };

            var userComments = await _commentService.GetCommentsByUserIdAsync(user.Id);

            var commentVMs = userComments.Select(c => new CommentViewModel
            {
                TeamId = c.TeamId,
                TeamName = c.Team?.TeamName ?? "Bilinmeyen",
                Content = c.Content,
                CreatedAt = c.CreatedAt
            }).ToList();

            ViewBag.UserComments = commentVMs;

            // Sayfalama bilgileri ViewBag ile View'a gönderiyoruz
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)teams.Count() / pageSize);

            return View(model);
        }



        [Authorize]
        [HttpGet("edit")]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return NotFound();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Image = user.Image
            };

            return View(model);
        }

        [Authorize]
        [HttpPost("edit")]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound();

            // 📷 Eğer dosya geldiyse işle
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // 5MB sınır kontrolü
                if (model.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "Yüklenen dosya en fazla 5 MB olmalıdır.");
                    return View(model);
                }

                // Uzantı kontrolü
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Sadece JPG, JPEG veya PNG dosyaları yükleyebilirsiniz.");
                    return View(model);
                }

                // Yükleme işlemi
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                user.Image = "/images/users/" + uniqueFileName;
            }

            // Diğer kullanıcı bilgileri güncelleniyor
            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // Kullanıcı oturumunu yenile
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("Profile", new { username = user.UserName });
        }
    }
}
