using DreamEleven.Entities;

namespace DreamEleven.DataAccess.Abstract
{
    // Yoruma özel veritabanı işlemleri için interface
    public interface ICommentRepository
    {
        Task AddCommentAsync(Comment comment);                       // Yorum ekler
        Task UpdateCommentAsync(Comment comment);                    // Yorum günceller
        Task DeleteCommentAsync(int id);                             // Yorum siler

        Task<Comment?> GetCommentByIdAsync(int id);                  // Belirli bir ID'ye göre yorum getirir
        Task<List<Comment>> GetCommentsByUserIdAsync(string userId); // Belirli bir kullanıcıya ait yorumları getirir
    }
}
