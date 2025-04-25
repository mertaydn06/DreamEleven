using DreamEleven.Entities;
namespace DreamEleven.DataAccess.Abstract
{
    public interface ICommentRepository
    {
        Task AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int id);
        Task<Comment?> GetCommentByIdAsync(int id);
        Task<List<Comment>> GetCommentsByUserIdAsync(string userId);
    }
}
