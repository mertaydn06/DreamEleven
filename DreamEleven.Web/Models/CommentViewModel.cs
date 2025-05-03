public class CommentViewModel
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public string UserName { get; set; } = string.Empty;
    public string UserImage { get; set; } = "/images/User.jpg";

    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
}
