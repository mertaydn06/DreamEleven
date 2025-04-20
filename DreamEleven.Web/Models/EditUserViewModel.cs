using System.ComponentModel.DataAnnotations;

public class EditUserViewModel
{
    [Required]
    public required string Username { get; set; }

    public required string Email { get; set; }

    public string? Image { get; set; }

    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }
}
