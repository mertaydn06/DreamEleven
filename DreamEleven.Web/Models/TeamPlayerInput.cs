namespace DreamEleven.Web.Models
{

    public class TeamPlayerInput
    {
        public string PositionSlot { get; set; } = string.Empty;  // Örn: "CM1"
        public int PlayerId { get; set; }                         // Seçilen oyuncunun ID'si
    }
}