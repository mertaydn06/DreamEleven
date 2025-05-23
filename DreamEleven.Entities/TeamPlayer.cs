namespace DreamEleven.Entities
{
    public class TeamPlayer
    {
        public int Id { get; set; }

        public int TeamId { get; set; }
        public int PlayerId { get; set; }

        // Navigation Properties
        public Team Team { get; set; } = null!;  // Takım bilgisi
        public Player Player { get; set; } = null!;  // Oyuncu bilgisi

        public string PositionSlot { get; set; } = string.Empty;  // Örnek: "ST1", "CM3", "GK", "CB2" — hangi kutuda yer aldığını anlamak için
    }
}
