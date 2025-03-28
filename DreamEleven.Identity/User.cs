using DreamEleven.Entities;
using Microsoft.AspNetCore.Identity;

namespace DreamEleven.Identity
{
    public class User : IdentityUser
    {
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Property
        public ICollection<Team> Teams { get; set; } = new List<Team>();  // Kullanıcının sahip olduğu takımlar
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();  // Kullanıcının yazdığı yorumlar
    }
}
