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
        private readonly UserManager<User> _userManager;
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService, UserManager<User> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int teamId, string content)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Geçersiz yorum.");
            }

            var comment = new Comment  // Comment sınıfından nesne oluşturarak değerleri verdik.
            {
                TeamId = teamId,
                UserId = user.Id,
                Content = content.Trim(),  // Yorumun başındaki ve sonundaki boşlukları sildik.
                CreatedAt = DateTime.Now
            };

            await _commentService.AddCommentAsync(comment);

            return RedirectToAction("Details", "Team", new { id = teamId });
        }


        [HttpGet]
        public async Task<IActionResult> EditComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);

            var user = await _userManager.GetUserAsync(User);

            if (comment == null || comment.UserId != user?.Id)
                return Unauthorized();

            return View(comment);
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(Comment model)
        {
            var user = await _userManager.GetUserAsync(User);

            var comment = await _commentService.GetCommentByIdAsync(model.Id);

            if (comment == null || comment.UserId != user?.Id)
                return Unauthorized();

            comment.Content = model.Content;
            await _commentService.UpdateCommentAsync(comment);

            return RedirectToAction("Details", "Team", new { id = comment.TeamId });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);

            var user = await _userManager.GetUserAsync(User);

            if (comment == null || comment.UserId != user?.Id)
                return Unauthorized();

            await _commentService.DeleteCommentAsync(id);
            return RedirectToAction("Details", "Team", new { id = comment.TeamId });
        }
    }
}
