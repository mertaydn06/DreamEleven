namespace DreamEleven.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Overall { get; set; }
        public string RealTeam { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public PositionType Position { get; set; }

        public enum PositionType
        {
            Goalkeeper, Defender, Midfielder, Forward
        }

        // Navigation Property
        public ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();  // Oyuncunun dahil olduğu tüm takımlar
    }
}
