using DreamEleven.Entities;
using DreamEleven.Identity;

namespace DreamEleven.Web.Models
{
    public class UserProfileViewModel
    {
        public required User User { get; set; }
        public List<Team> Teams { get; set; } = new();
        public bool IsCurrentUser { get; set; }  // kendisi mi bakıyor profiline
    }
}
