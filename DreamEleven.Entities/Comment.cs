namespace DreamEleven.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int TeamId { get; set; }  // Yorumun ait olduğu takım
        public string UserId { get; set; } = string.Empty;  // Yorumun yazıldığı kullanıcı
        public string Content { get; set; } = string.Empty;  // Yorumun içeriği
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public Team Team { get; set; } = null!;  // Yorumun ait olduğu takım

        // public User User { get; set; }  // "User" navigation eklenmiyor çünkü bu Entities bağımsız olmalıdır bu yüzden katmanının Identity'e bağımlı kalmaması için sadece UserId tutuyoruz.
    }
}
