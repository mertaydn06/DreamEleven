using DreamEleven.Business.Abstract;
using DreamEleven.Entities;
using DreamEleven.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DreamEleven.Web.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly UserManager<User> _userManager;    // Kullanıcı yönetimi için UserManager servisi
        private readonly ICommentService _commentService;   // Yorum işlemleri için ICommentService

        public CommentController(UserManager<User> userManager, ICommentService commentService)
        {
            _userManager = userManager;         // UserManager servisi sınıfa atanır
            _commentService = commentService;   // Yorum servisi sınıfa atanır
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(int teamId, string content)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || string.IsNullOrWhiteSpace(content))  // Eğer kullanıcı yoksa veya içerik boşsa
            {
                return BadRequest("Geçersiz yorum.");
            }

            var comment = new Comment  // Comment sınıfından nesne oluşturarak değerleri verdik.
            {
                TeamId = teamId,            // Yorumun ait olduğu takımın ID'si
                UserId = user.Id,           // Yorum yapan kullanıcının ID'si
                Content = content.Trim(),   // Yorumun başındaki ve sonundaki boşukları siler.
                CreatedAt = DateTime.Now
            };

            await _commentService.AddCommentAsync(comment);

            return RedirectToAction("Details", "Team", new { id = teamId });  // Takımın detay sayfasına yönlendirir.
        }


        [HttpGet]
        public async Task<IActionResult> EditComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);

            var user = await _userManager.GetUserAsync(User);  // Geçerli oturumdaki kullanıcıyı alır.

            if (comment == null || comment.UserId != user?.Id)  // Eğer yorum bulunamazsa ya da yorumun sahibi geçerli kullanıcı değilse yetkilendirme hatası verilir.

                return Unauthorized();

            return View(comment);  // Yorum bulunduysa Edit formuna yönlendirilir
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(Comment model)
        {
            var user = await _userManager.GetUserAsync(User);  // Geçerli oturumdaki kullanıcıyı alır.

            var comment = await _commentService.GetCommentByIdAsync(model.Id);  // Yorumu veritabanından getirir.

            if (comment == null || comment.UserId != user?.Id)  // Eğer yorum bulunamazsa ya da yorumun sahibi geçerli kullanıcı değilse yetkilendirme hatası verilir.

                return Unauthorized();


            comment.Content = model.Content;

            await _commentService.UpdateCommentAsync(comment);

            return RedirectToAction("Details", "Team", new { id = comment.TeamId });  // Takım detay sayfasına yönlendirir.
        }


        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);  // Yorumun veritabanından getirilir.

            var user = await _userManager.GetUserAsync(User);  // Geçerli oturumdaki kullanıcıyı alır.

            if (comment == null || comment.UserId != user?.Id)
                return Unauthorized();

            await _commentService.DeleteCommentAsync(id);

            return RedirectToAction("Details", "Team", new { id = comment.TeamId }); // Yorum silindikten sonra takım detay sayfasına yönlendirir.
        }
    }
}
