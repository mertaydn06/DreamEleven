using DreamEleven.Business.Abstract;
using DreamEleven.DataAccess.Abstract;
using DreamEleven.Entities;

namespace DreamEleven.Business.Concrete
{
    public class CommentManager : ICommentService  // ICommentService'i uygulayan sınıf — Controller buradan çağırır
    {
        private readonly ICommentRepository _commentRepository;

        public CommentManager(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;  // Dependency Injection (DI) kullanılarak repository sınıfı enjekte ediliyor
        }

        // Yorum ekler
        public async Task AddCommentAsync(Comment comment)
        {
            await _commentRepository.AddCommentAsync(comment);
        }

        // Yorum günceller
        public async Task UpdateCommentAsync(Comment comment)
        {
            await _commentRepository.UpdateCommentAsync(comment);
        }

        // Yorum siler
        public async Task DeleteCommentAsync(int id)
        {
            await _commentRepository.DeleteCommentAsync(id);
        }


        // ID'ye göre yorum getirir
        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _commentRepository.GetCommentByIdAsync(id);
        }

        // Kullanıcıya ait tüm yorumları getirir
        public async Task<List<Comment>> GetCommentsByUserIdAsync(string userId)
        {
            return await _commentRepository.GetCommentsByUserIdAsync(userId);
        }
    }
}
