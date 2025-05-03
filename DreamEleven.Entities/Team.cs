namespace DreamEleven.Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string Formation { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();  // Takımda ki oyuncular

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();  // Takımın yorumları

        // public User User { get; set; }  // "User" navigation eklenmiyor çünkü bu Entities bağımsız olmalıdır bu yüzden katmanının Identity'e bağımlı kalmaması için sadece UserId tutuyoruz.
    }
}
