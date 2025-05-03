using DreamEleven.DataAccess.Abstract;
using DreamEleven.Entities;
using Microsoft.EntityFrameworkCore;

namespace DreamEleven.DataAccess.Concrete
{
    public class EfCommentRepository : ICommentRepository
    {
        private readonly DreamElevenDbContext _context;          // Veritabanı erişimi için DbContext

        public EfCommentRepository(DreamElevenDbContext context)
        {
            _context = context;  // Dependency Injection (DI) kullanılarak DbContext enjekte ediliyor
        }

        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            _context.Comments.Update(comment);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.Team)                             // Yorumun ait olduğu takımı da yükler
                .FirstOrDefaultAsync(c => c.Id == id);            // ID'si verilen yorumu getirir
        }

        public async Task<List<Comment>> GetCommentsByUserIdAsync(string userId)
        {
            return await _context.Comments
                .Where(c => c.UserId == userId)                  // Belirtilen userId'ye ait yorumları filtreler
                .Include(c => c.Team)                            // Yorumların ait olduğu takımları da yükler
                .OrderByDescending(c => c.CreatedAt)             // Yorumları oluşturulma tarihine göre azalan sırayla sıralar
                .ToListAsync();                                  // Yorumları liste olarak döndürür
        }
    }
}
