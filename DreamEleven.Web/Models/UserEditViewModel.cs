public class UserEditViewModel
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? Image { get; set; }

    public IFormFile? ImageFile { get; set; }
}
